namespace FORGE.Compilation.Exceptions;

/// <summary>
/// Thrown when the Roslyn pipeline fails to compile or validate LLM-generated C# source code.
/// </summary>
public sealed class CompilationException : Exception
{
    /// <summary>
    /// Ordered list of diagnostic messages produced by the compiler or AST validator.
    /// </summary>
    public IReadOnlyList<string> Diagnostics { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="CompilationException"/>.
    /// </summary>
    /// <param name="message">Summary message.</param>
    /// <param name="diagnostics">Compiler or validation diagnostics.</param>
    public CompilationException(string message, IReadOnlyList<string> diagnostics)
        : base(message)
    {
        Diagnostics = diagnostics;
    }
}
