using FluentAssertions;
using FORGE.Compilation.Exceptions;

namespace FORGE.Tests.Compilation;

public class CompilationExceptionTests
{
    [Fact]
    public void Constructor_SetsMessageAndDiagnostics()
    {
        var diagnostics = new[] { "error CS0001: Type not found", "error CS0002: Missing member" };

        var ex = new CompilationException("Compilation failed.", diagnostics);

        ex.Message.Should().Be("Compilation failed.");
        ex.Diagnostics.Should().BeEquivalentTo(diagnostics);
    }

    [Fact]
    public void Constructor_EmptyDiagnostics_DiagnosticsIsEmpty()
    {
        var ex = new CompilationException("No strategy found.", []);

        ex.Diagnostics.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_DiagnosticsIsReadOnly()
    {
        var ex = new CompilationException("Failed.", ["error"]);

        ex.Diagnostics.Should().BeAssignableTo<IReadOnlyList<string>>();
    }
}
