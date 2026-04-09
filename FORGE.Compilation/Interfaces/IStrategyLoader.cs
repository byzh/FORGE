using FORGE.Core.Interfaces;

namespace FORGE.Compilation.Interfaces;

/// <summary>
/// Orchestrates the full pipeline from a natural-language strategy description
/// to a live <see cref="IStrategy"/> instance ready for execution.
/// </summary>
public interface IStrategyLoader
{
    /// <summary>
    /// Generates, compiles, and instantiates an <see cref="IStrategy"/> from a plain-English description.
    /// </summary>
    /// <param name="prompt">
    /// Natural-language description of the strategy
    /// (e.g. "Sell a put on TSLA when IV Rank exceeds 60").
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A live <see cref="IStrategy"/> instance loaded in an isolated sandbox.</returns>
    /// <exception cref="Exceptions.CompilationException">
    /// Thrown when the generated code fails AST validation or Roslyn compilation.
    /// </exception>
    Task<IStrategy> LoadFromPromptAsync(string prompt, CancellationToken ct = default);
}
