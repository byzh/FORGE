using FORGE.Compilation.Interfaces;
using FORGE.Core.Interfaces;

namespace FORGE.Compilation;

/// <summary>
/// Implements <see cref="IStrategyLoader"/> by orchestrating the full code-generation
/// and compilation pipeline: LLM → C# source → Roslyn → live <see cref="IStrategy"/>.
/// </summary>
public sealed class StrategyLoader : IStrategyLoader
{
    private readonly ICodeGenerator _codeGenerator;
    private readonly IRoslynCompiler _compiler;

    /// <summary>
    /// Initializes a new instance of <see cref="StrategyLoader"/>.
    /// </summary>
    /// <param name="codeGenerator">LLM-backed code generator (Claude, Gemini, etc.).</param>
    /// <param name="compiler">Roslyn compilation pipeline.</param>
    public StrategyLoader(ICodeGenerator codeGenerator, IRoslynCompiler compiler)
    {
        _codeGenerator = codeGenerator;
        _compiler = compiler;
    }

    /// <inheritdoc/>
    public async Task<IStrategy> LoadFromPromptAsync(string prompt, CancellationToken ct = default)
    {
        var sourceCode = await _codeGenerator.GenerateAsync(prompt, ct);
        return await _compiler.CompileAsync(sourceCode, ct);
    }
}
