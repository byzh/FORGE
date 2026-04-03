using FORGE.Core.Enums;

namespace FORGE.Core;

/// <summary>
/// Typed output produced by a financial agent after analysing a <see cref="MarketContext"/>.
/// </summary>
/// <param name="Direction">The directional bias of the signal.</param>
/// <param name="Confidence">How strongly the agent supports this signal.</param>
/// <param name="Rationale">Human-readable explanation of the agent's reasoning.</param>
/// <param name="Metrics">
/// Named numeric metrics that support the signal (e.g. "Delta", "IVRank", "VIX").
/// </param>
public record AgentSignal(
    SignalDirection Direction,
    ConfidenceLevel Confidence,
    string Rationale,
    IReadOnlyDictionary<string, decimal> Metrics);
