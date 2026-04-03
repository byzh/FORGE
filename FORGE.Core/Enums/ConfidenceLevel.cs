namespace FORGE.Core.Enums;

/// <summary>
/// Represents the confidence level an agent assigns to its signal or a validator assigns to its result.
/// </summary>
public enum ConfidenceLevel
{
    /// <summary>Signal is weakly supported — treat with caution.</summary>
    Low,

    /// <summary>Signal is moderately supported by available data.</summary>
    Medium,

    /// <summary>Signal is strongly supported — multiple indicators agree.</summary>
    High
}
