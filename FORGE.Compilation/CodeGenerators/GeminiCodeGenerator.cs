using System.Text;
using System.Text.Json;
using FORGE.Compilation.Interfaces;
using FORGE.Compilation.Options;
using Microsoft.Extensions.Options;

namespace FORGE.Compilation.CodeGenerators;

/// <summary>
/// Implements <see cref="ICodeGenerator"/> using the Google Gemini REST API.
/// Sends a natural-language strategy description and returns LLM-generated C# source code.
/// Configuration is bound from the <c>CodeGenerators:Gemini</c> appsettings section.
/// </summary>
public sealed class GeminiCodeGenerator : ICodeGenerator
{
    private readonly HttpClient _http;
    private readonly GeminiOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="GeminiCodeGenerator"/>.
    /// </summary>
    /// <param name="http">
    /// <see cref="HttpClient"/> instance (injected via IHttpClientFactory in production).
    /// </param>
    /// <param name="options">Gemini configuration bound from appsettings.json.</param>
    public GeminiCodeGenerator(HttpClient http, IOptions<GeminiOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    /// <inheritdoc/>
    public async Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
    {
        // Gemini API key is passed as a query parameter, not a header
        var url = $"{_options.ApiUrl}/{_options.Model}:generateContent?key={_options.ApiKey}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = BuildSystemPrompt(prompt) }
                    }
                }
            },
            generationConfig = new
            {
                maxOutputTokens = _options.MaxTokens
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(
            JsonSerializer.Serialize(requestBody),
            Encoding.UTF8,
            "application/json");

        using var response = await _http.SendAsync(request, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        return ExtractCode(json);
    }

    private static string BuildSystemPrompt(string userPrompt) =>
        $$"""
        You are a C# code generator for a financial trading strategy system.
        Generate a single C# class that implements the IStrategy interface from the FORGE.Core namespace.
        Return ONLY the raw C# source code, no markdown, no explanation.

        IStrategy contract:
        - string Name { get; }
        - string Underlying { get; }
        - Task<AgentSignal> GenerateSignalAsync(MarketContext context, CancellationToken ct)
        - bool Validate(out IReadOnlyList<string> violations)

        Strategy description:
        {{userPrompt}}
        """;

    private static string ExtractCode(string responseJson)
    {
        using var doc = JsonDocument.Parse(responseJson);
        return doc.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}
