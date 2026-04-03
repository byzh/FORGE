namespace FORGE.Core.Interfaces;

/// <summary>
/// Contract for every specialized LLM-backed financial analysis agent.
/// Each agent encapsulates a distinct analytical lens (macro, technical, risk).
/// </summary>
public interface IFinancialAgent
{
    /// <summary>
    /// Display name identifying the agent's analytical role (e.g. "MacroAgent").
    /// </summary>
    string Role { get; }

    /// <summary>
    /// Analyses the provided market context and returns a typed trading signal.
    /// </summary>
    /// <param name="context">Current market snapshot to evaluate.</param>
    /// <param name="ct">Cancellation token for cooperative cancellation of the LLM call.</param>
    /// <returns>An <see cref="AgentSignal"/> containing direction, confidence, and supporting metrics.</returns>
    Task<AgentSignal> AnalyzeAsync(MarketContext context, CancellationToken ct);
}
