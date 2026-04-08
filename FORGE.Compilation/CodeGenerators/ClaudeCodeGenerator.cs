using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FORGE.Compilation.Interfaces;
using FORGE.Compilation.Options;
using Microsoft.Extensions.Options;

namespace FORGE.Compilation.CodeGenerators;

/// <summary>
/// Implements <see cref="ICodeGenerator"/> using the Anthropic Claude REST API.
/// Sends a natural-language strategy description and returns LLM-generated C# source code.
/// Configuration is bound from the <c>CodeGenerators:Claude</c> appsettings section.
/// </summary>
public sealed class ClaudeCodeGenerator : ICodeGenerator
{
    private readonly HttpClient _http;
    private readonly ClaudeOptions _options;

    /// <summary>
    /// Initializes a new instance of <see cref="ClaudeCodeGenerator"/>.
    /// </summary>
    /// <param name="http">
    /// <see cref="HttpClient"/> instance (injected via IHttpClientFactory in production).
    /// </param>
    /// <param name="options">Claude configuration bound from appsettings.json.</param>
    public ClaudeCodeGenerator(HttpClient http, IOptions<ClaudeOptions> options)
    {
        _http = http;
        _options = options.Value;
    }

    /// <inheritdoc/>
    public async Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
    {
        var requestBody = new
        {
            model = _options.Model,
            max_tokens = _options.MaxTokens,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = BuildSystemPrompt(prompt)
                }
            }
        };

        using var request = new HttpRequestMessage(HttpMethod.Post, _options.ApiUrl);
        request.Headers.Add("x-api-key", _options.ApiKey);
        request.Headers.Add("anthropic-version", _options.ApiVersion);
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
            .GetProperty("content")[0]
            .GetProperty("text")
            .GetString() ?? string.Empty;
    }
}
