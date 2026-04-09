using FluentAssertions;
using FORGE.Compilation;
using FORGE.Compilation.Exceptions;
using FORGE.Compilation.Interfaces;
using FORGE.Core.Interfaces;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace FORGE.Tests.Compilation;

public class StrategyLoaderTests
{
    private readonly ICodeGenerator _codeGenerator = Substitute.For<ICodeGenerator>();
    private readonly IRoslynCompiler _compiler = Substitute.For<IRoslynCompiler>();
    private readonly StrategyLoader _sut;

    public StrategyLoaderTests()
    {
        _sut = new StrategyLoader(_codeGenerator, _compiler);
    }

    [Fact]
    public async Task LoadFromPromptAsync_ValidPipeline_ReturnsIStrategyInstance()
    {
        const string prompt = "Sell a put on TSLA when IV Rank exceeds 60";
        const string generatedCode = "public class TslaShortPut : IStrategy { }";
        var expectedStrategy = Substitute.For<IStrategy>();

        _codeGenerator.GenerateAsync(prompt, Arg.Any<CancellationToken>())
            .Returns(generatedCode);
        _compiler.CompileAsync(generatedCode, Arg.Any<CancellationToken>())
            .Returns(expectedStrategy);

        var result = await _sut.LoadFromPromptAsync(prompt);

        result.Should().Be(expectedStrategy);
    }

    [Fact]
    public async Task LoadFromPromptAsync_CodeGeneratorThrows_PropagatesException()
    {
        _codeGenerator.GenerateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .ThrowsAsync(new HttpRequestException("LLM API unavailable"));

        var act = () => _sut.LoadFromPromptAsync("any prompt");

        await act.Should().ThrowAsync<HttpRequestException>();
    }

    [Fact]
    public async Task LoadFromPromptAsync_CompilerThrows_PropagatesCompilationException()
    {
        const string generatedCode = "invalid code";
        _codeGenerator.GenerateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(generatedCode);
        _compiler.CompileAsync(generatedCode, Arg.Any<CancellationToken>())
            .ThrowsAsync(new CompilationException("AST validation failed.", ["Blocked namespace: System.IO"]));

        var act = () => _sut.LoadFromPromptAsync("some prompt");

        await act.Should().ThrowAsync<CompilationException>()
            .WithMessage("*AST validation failed*");
    }

    [Fact]
    public async Task LoadFromPromptAsync_PassesPromptToCodeGenerator()
    {
        const string prompt = "Buy a call on QQQ";
        var strategy = Substitute.For<IStrategy>();

        _codeGenerator.GenerateAsync(prompt, Arg.Any<CancellationToken>())
            .Returns("some code");
        _compiler.CompileAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(strategy);

        await _sut.LoadFromPromptAsync(prompt);

        await _codeGenerator.Received(1).GenerateAsync(prompt, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LoadFromPromptAsync_PassesGeneratedCodeToCompiler()
    {
        const string generatedCode = "public class MyStrategy : IStrategy { }";
        var strategy = Substitute.For<IStrategy>();

        _codeGenerator.GenerateAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(generatedCode);
        _compiler.CompileAsync(generatedCode, Arg.Any<CancellationToken>())
            .Returns(strategy);

        await _sut.LoadFromPromptAsync("any prompt");

        await _compiler.Received(1).CompileAsync(generatedCode, Arg.Any<CancellationToken>());
    }
}
