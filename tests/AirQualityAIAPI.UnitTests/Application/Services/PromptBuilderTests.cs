using AirQualityIAAPI.Application.DTOs;
using AirQualityIAAPI.Application.Services;
using AirQualityIAAPI.Domain.Entities;
using FluentAssertions;

namespace AirQualityAIAPI.UnitTests.Application.Services;

public class PromptBuilderTests
{
    [Fact]
    public void Build_ShouldIncludeRulesCurrentDataHistoryAndCurrentQuestion()
    {
        var request = new ChatRequestDto
        {
            SessionId = "s-1",
            Message = "¿Debo usar tapabocas hoy?",
            AirData = new AirQualityDataDto
            {
                PM25 = 20,
                PM10 = 45,
                CO = 0.4,
                O3 = 0.03,
                Temperature = 19
            }
        };

        var history = new List<ChatMessage>
        {
            new()
            {
                UserMessage = "¿Cómo estuvo ayer?",
                AssistantResponse = "Fue moderada.",
                CreatedAt = DateTime.UtcNow.AddMinutes(-5)
            }
        };

        var prompt = PromptBuilder.Build(request, history);

        prompt.Should().Contain("Eres un asistente virtual especializado en calidad del aire en Bogotá.");
        prompt.Should().Contain("Responde siempre en español.");
        prompt.Should().Contain("DATOS ACTUALES");
        prompt.Should().Contain("PM2.5: 20");
        prompt.Should().Contain("PM10: 45");
        prompt.Should().Contain("CO: 0.4");
        prompt.Should().Contain("O3: 0.03");
        prompt.Should().Contain("Temperatura: 19");
        prompt.Should().Contain("HISTORIAL");
        prompt.Should().Contain("Usuario: ¿Cómo estuvo ayer?");
        prompt.Should().Contain("Asistente: Fue moderada.");
        prompt.Should().Contain("Pregunta actual: ¿Debo usar tapabocas hoy?");
    }
}
