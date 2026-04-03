namespace FORGE.Core.Enums;

/// <summary>
/// Represents the directional bias of a trading signal produced by a financial agent.
/// </summary>
public enum SignalDirection
{
    /// <summary>Positive outlook — expect price appreciation or volatility contraction.</summary>
    Bullish,

    /// <summary>Negative outlook — expect price depreciation or volatility expansion.</summary>
    Bearish,

    /// <summary>No directional conviction — agent abstains from a directional call.</summary>
    Neutral
}
