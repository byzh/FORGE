namespace FORGE.Compilation.Interfaces;

/// <summary>
/// Converts a natural-language strategy description into compilable C# source code
/// by invoking an LLM code-generation model.
/// </summary>
public interface ICodeGenerator
{
    /// <summary>
    /// Generates a C# class implementing <c>IStrategy</c> from a plain-English description.
    /// </summary>
    /// <param name="prompt">
    /// Natural-language description of the strategy
    /// (e.g. "Sell a put on TSLA when IV Rank exceeds 60").
    /// </param>
    /// <param name="ct">Cancellation token for cooperative cancellation of the LLM call.</param>
    /// <returns>
    /// A string containing valid C# source code that implements <c>IStrategy</c>,
    /// ready to be passed to the Roslyn compilation pipeline.
    /// </returns>
    Task<string> GenerateAsync(string prompt, CancellationToken ct = default);
}
