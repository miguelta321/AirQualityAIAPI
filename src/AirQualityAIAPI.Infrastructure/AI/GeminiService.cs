using System.Text;
using System.Text.Json;
using AirQualityIAAPI.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace AirQualityIAAPI.Infrastructure.AI;

public class GeminiService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly GeminiSettings _settings;

    public GeminiService(
        HttpClient httpClient,
        IOptions<GeminiSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }

    public async Task<string> GenerateResponseAsync(string prompt)
    {
        var body = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = prompt
                        }
                    }
                }
            }
        };

        var json = JsonSerializer.Serialize(body);

        var response = await _httpClient.PostAsync(
            $"models/gemini-2.5-flash-lite:generateContent?key={_settings.ApiKey}",
            new StringContent(
                json,
                Encoding.UTF8,
                "application/json"));

        var content =
            await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"Gemini Error: {content}");
        }

        using var document =
            JsonDocument.Parse(content);

        return document
            .RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString()
            ?? string.Empty;
    }
}