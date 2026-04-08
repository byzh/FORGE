namespace FORGE.Compilation.Options;

/// <summary>
/// Configuration options for <c>ClaudeCodeGenerator</c>, bound from the
/// <c>CodeGenerators:Claude</c> section of appsettings.json.
/// </summary>
public sealed class ClaudeOptions
{
    /// <summary>The appsettings section key for this options class.</summary>
    public const string SectionName = "CodeGenerators:Claude";

    /// <summary>Anthropic API key. Must be supplied via environment variable or secret store — never hardcoded.</summary>
    public string ApiKey { get; init; } = string.Empty;

    /// <summary>Claude model identifier (e.g. "claude-sonnet-4-5").</summary>
    public string Model { get; init; } = "claude-sonnet-4-5";

    /// <summary>Anthropic Messages API endpoint URL.</summary>
    public string ApiUrl { get; init; } = "https://api.anthropic.com/v1/messages";

    /// <summary>Anthropic API version header value.</summary>
    public string ApiVersion { get; init; } = "2023-06-01";

    /// <summary>Maximum number of tokens in the LLM response.</summary>
    public int MaxTokens { get; init; } = 4096;
}
