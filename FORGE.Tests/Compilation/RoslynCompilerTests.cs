using FluentAssertions;
using FORGE.Compilation;
using FORGE.Compilation.Exceptions;
using FORGE.Core.Interfaces;

namespace FORGE.Tests.Compilation;

public class RoslynCompilerTests
{
    private readonly RoslynCompiler _sut = new();

    // Minimal valid IStrategy implementation used across happy-path tests
    private const string ValidStrategySource = """
        using System;
        using System.Collections.Generic;
        using System.Threading;
        using System.Threading.Tasks;
        using FORGE.Core;
        using FORGE.Core.Enums;
        using FORGE.Core.Interfaces;

        public class TestStrategy : IStrategy
        {
            public string Name => "TestStrategy";
            public string Underlying => "QQQ";

            public Task<AgentSignal> GenerateSignalAsync(MarketContext context, CancellationToken ct)
            {
                var signal = new AgentSignal(
                    SignalDirection.Neutral,
                    ConfidenceLevel.Low,
                    "Test rationale",
                    new Dictionary<string, decimal>());
                return Task.FromResult(signal);
            }

            public bool Validate(out IReadOnlyList<string> violations)
            {
                violations = Array.Empty<string>();
                return true;
            }
        }
        """;

    [Fact]
    public async Task CompileAsync_ValidStrategySource_ReturnsIStrategyInstance()
    {
        var result = await _sut.CompileAsync(ValidStrategySource);

        result.Should().NotBeNull();
        result.Should().BeAssignableTo<IStrategy>();
        result.Name.Should().Be("TestStrategy");
        result.Underlying.Should().Be("QQQ");
    }

    [Fact]
    public async Task CompileAsync_BlockedNamespace_ThrowsCompilationException()
    {
        var sourceWithBlockedNs = ValidStrategySource.Replace(
            "using System;",
            "using System;\nusing System.IO;");

        var act = () => _sut.CompileAsync(sourceWithBlockedNs);

        await act.Should().ThrowAsync<CompilationException>()
            .WithMessage("*AST validation failed*");
    }

    [Fact]
    public async Task CompileAsync_SyntaxError_ThrowsCompilationException()
    {
        const string brokenSource = "public class Broken { this is not valid C# }";

        var act = () => _sut.CompileAsync(brokenSource);

        await act.Should().ThrowAsync<CompilationException>()
            .WithMessage("*Roslyn compilation failed*");
    }

    [Fact]
    public async Task CompileAsync_NoIStrategyImplementation_ThrowsCompilationException()
    {
        const string noStrategySource = """
            public class NotAStrategy
            {
                public string Hello => "world";
            }
            """;

        var act = () => _sut.CompileAsync(noStrategySource);

        await act.Should().ThrowAsync<CompilationException>()
            .WithMessage("*No concrete IStrategy implementation*");
    }

    [Fact]
    public async Task CompileAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();

        var act = () => _sut.CompileAsync(ValidStrategySource, cts.Token);

        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task CompileAsync_MultipleBlockedNamespaces_DiagnosticsContainAllViolations()
    {
        var sourceWithMultipleBlocked = ValidStrategySource.Replace(
            "using System;",
            "using System;\nusing System.IO;\nusing System.Net;");

        var act = () => _sut.CompileAsync(sourceWithMultipleBlocked);

        var exception = await act.Should().ThrowAsync<CompilationException>();
        exception.Which.Diagnostics.Should().HaveCountGreaterThanOrEqualTo(2);
    }
}
