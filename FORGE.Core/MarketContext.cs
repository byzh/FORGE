using FORGE.Core.Enums;

namespace FORGE.Core;

/// <summary>
/// Immutable snapshot of market conditions passed to every financial agent for analysis.
/// </summary>
/// <param name="Underlying">The ticker symbol being analysed (e.g. "TSLA", "QQQ").</param>
/// <param name="SpotPrice">Current market price of the underlying asset.</param>
/// <param name="IVRank">
/// Implied Volatility Rank over the trailing 52 weeks, expressed as a percentage (0–100).
/// </param>
/// <param name="VIX">Current CBOE Volatility Index value, used as a macro fear gauge.</param>
/// <param name="Timestamp">UTC timestamp at which this context was captured.</param>
public record MarketContext(
    string Underlying,
    decimal SpotPrice,
    decimal IVRank,
    decimal VIX,
    DateTime Timestamp);
