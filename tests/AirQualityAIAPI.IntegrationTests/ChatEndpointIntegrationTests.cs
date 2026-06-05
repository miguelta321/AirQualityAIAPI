using System.Net;
using System.Net.Http.Json;
using AirQualityIAAPI.Application.DTOs;
using AirQualityIAAPI.Application.Interfaces;
using AirQualityIAAPI.Domain.Entities;
using AirQualityIAAPI.Domain.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AirQualityAIAPI.IntegrationTests;

public class ChatEndpointIntegrationTests
    : IClassFixture<ChatEndpointIntegrationTests.TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ChatEndpointIntegrationTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostChat_ShouldReturnOkAndMockedAiResponse()
    {
        var request = CreateRequest("session-ok", "¿Cómo está la calidad del aire?");

        var response = await _client.PostAsJsonAsync("/api/chat", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ChatResponseDto>();
        body.Should().NotBeNull();
        body!.Response.Should().Be("RESPUESTA_BASE");
    }

    [Fact]
    public async Task PostChat_WithInvalidPayload_ShouldReturnBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/chat", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task PostChat_ShouldKeepConversationContextBySession()
    {
        var sessionId = "session-context";

        var firstResponse = await _client.PostAsJsonAsync(
            "/api/chat",
            CreateRequest(sessionId, "Primer mensaje"));
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var secondResponse = await _client.PostAsJsonAsync(
            "/api/chat",
            CreateRequest(sessionId, "Segundo mensaje"));
        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await secondResponse.Content.ReadFromJsonAsync<ChatResponseDto>();
        body.Should().NotBeNull();
        body!.Response.Should().Be("CONTEXT_PRESENT");
    }

    private static ChatRequestDto CreateRequest(string sessionId, string message)
    {
        return new ChatRequestDto
        {
            SessionId = sessionId,
            Message = message,
            AirData = new AirQualityDataDto
            {
                PM25 = 12,
                PM10 = 24,
                CO = 0.3,
                O3 = 0.02,
                Temperature = 21
            }
        };
    }

    public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");

            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IChatRepository>();
                services.RemoveAll<IAIService>();

                services.AddSingleton<IChatRepository, InMemoryChatRepository>();
                services.AddSingleton<IAIService, FakeAiService>();
            });
        }
    }

    private sealed class InMemoryChatRepository : IChatRepository
    {
        private readonly List<ChatMessage> _messages = [];
        private readonly object _lock = new();

        public Task<List<ChatMessage>> GetHistoryAsync(string sessionId)
        {
            lock (_lock)
            {
                var result = _messages
                    .Where(x => x.SessionId == sessionId)
                    .OrderByDescending(x => x.CreatedAt)
                    .Take(10)
                    .OrderBy(x => x.CreatedAt)
                    .ToList();

                return Task.FromResult(result);
            }
        }

        public Task SaveAsync(ChatMessage message)
        {
            lock (_lock)
            {
                _messages.Add(message);
            }

            return Task.CompletedTask;
        }
    }

    private sealed class FakeAiService : IAIService
    {
        public Task<string> GenerateResponseAsync(string prompt)
        {
            if (prompt.Contains("Usuario: Primer mensaje", StringComparison.Ordinal))
            {
                return Task.FromResult("CONTEXT_PRESENT");
            }

            return Task.FromResult("RESPUESTA_BASE");
        }
    }
}
