using System.Net;
using System.Text;
using AirQualityIAAPI.Infrastructure.AI;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace AirQualityAIAPI.UnitTests.Infrastructure.AI;

public class GeminiServiceTests
{
    [Fact]
    public async Task GenerateResponseAsync_ShouldParseGeminiResponse()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    {
                      "candidates": [
                        {
                          "content": {
                            "parts": [
                              { "text": "Respuesta generada" }
                            ]
                          }
                        }
                      ]
                    }
                    """,
                    Encoding.UTF8,
                    "application/json")
            });

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/")
        };

        var settings = Options.Create(new GeminiSettings { ApiKey = "fake-key" });
        var sut = new GeminiService(client, settings);

        var result = await sut.GenerateResponseAsync("prompt");

        result.Should().Be("Respuesta generada");
    }

    [Fact]
    public async Task GenerateResponseAsync_WhenApiFails_ShouldThrowExceptionWithBody()
    {
        var handler = new StubHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("error-body", Encoding.UTF8, "application/json")
            });

        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test/")
        };

        var settings = Options.Create(new GeminiSettings { ApiKey = "fake-key" });
        var sut = new GeminiService(client, settings);

        var act = () => sut.GenerateResponseAsync("prompt");

        await act.Should().ThrowAsync<Exception>()
            .WithMessage("*Gemini Error: error-body*");
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

        public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        {
            _responseFactory = responseFactory;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_responseFactory(request));
        }
    }
}
