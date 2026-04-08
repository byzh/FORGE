namespace FORGE.Compilation.Options;

/// <summary>
/// Configuration options for <c>GeminiCodeGenerator</c>, bound from the
/// <c>CodeGenerators:Gemini</c> section of appsettings.json.
/// </summary>
public sealed class GeminiOptions
{
    /// <summary>The appsettings section key for this options class.</summary>
    public const string SectionName = "CodeGenerators:Gemini";

    /// <summary>Google Gemini API key. Must be supplied via environment variable or secret store — never hardcoded.</summary>
    public string ApiKey { get; init; } = string.Empty;

    /// <summary>Gemini model identifier (e.g. "gemini-2.0-flash").</summary>
    public string Model { get; init; } = "gemini-2.0-flash";

    /// <summary>Gemini generateContent API base URL (model name is appended at runtime).</summary>
    public string ApiUrl { get; init; } = "https://generativelanguage.googleapis.com/v1beta/models";

    /// <summary>Maximum number of tokens in the LLM response.</summary>
    public int MaxTokens { get; init; } = 4096;
}
