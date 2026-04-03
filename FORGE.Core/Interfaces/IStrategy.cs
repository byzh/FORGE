namespace FORGE.Core.Interfaces;

/// <summary>
/// Contract for a financial trading strategy that can generate signals from market data.
/// Each concrete strategy encapsulates its own parameters, entry logic, and self-validation.
/// </summary>
public interface IStrategy
{
    /// <summary>Unique display name of the strategy (e.g. "ShortPut", "PMCC").</summary>
    string Name { get; }

    /// <summary>The ticker symbol this strategy targets (e.g. "QQQ", "TSLA").</summary>
    string Underlying { get; }

    /// <summary>
    /// Generates a trading signal from the provided market context.
    /// </summary>
    /// <param name="context">Current market snapshot to evaluate.</param>
    /// <param name="ct">Cancellation token for cooperative cancellation.</param>
    /// <returns>An <see cref="AgentSignal"/> representing the strategy's recommendation.</returns>
    Task<AgentSignal> GenerateSignalAsync(MarketContext context, CancellationToken ct);

    /// <summary>
    /// Validates the strategy's own parameters for internal coherence
    /// (e.g. delta range, DTE bounds, strike feasibility).
    /// </summary>
    /// <param name="violations">
    /// When this method returns <see langword="false"/>, contains the list of validation
    /// failure messages; otherwise an empty list.
    /// </param>
    /// <returns><see langword="true"/> if all parameters are coherent; otherwise <see langword="false"/>.</returns>
    bool Validate(out IReadOnlyList<string> violations);
}
