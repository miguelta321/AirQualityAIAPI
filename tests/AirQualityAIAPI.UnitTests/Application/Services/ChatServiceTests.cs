using AirQualityIAAPI.Application.DTOs;
using AirQualityIAAPI.Application.Interfaces;
using AirQualityIAAPI.Application.Services;
using AirQualityIAAPI.Domain.Entities;
using AirQualityIAAPI.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace AirQualityAIAPI.UnitTests.Application.Services;

public class ChatServiceTests
{
    [Fact]
    public async Task ProcessMessageAsync_ShouldBuildPromptCallAiAndPersistMessage()
    {
        var request = new ChatRequestDto
        {
            SessionId = "session-123",
            Message = "¿Cómo está el aire hoy?",
            AirData = new AirQualityDataDto
            {
                PM25 = 14.2,
                PM10 = 35.8,
                CO = 0.6,
                O3 = 0.02,
                Temperature = 22.5
            }
        };

        var history = new List<ChatMessage>
        {
            new()
            {
                SessionId = request.SessionId,
                UserMessage = "Mensaje previo",
                AssistantResponse = "Respuesta previa",
                CreatedAt = DateTime.UtcNow.AddMinutes(-10)
            }
        };

        var chatRepositoryMock = new Mock<IChatRepository>();
        chatRepositoryMock
            .Setup(x => x.GetHistoryAsync(request.SessionId))
            .ReturnsAsync(history);

        var aiServiceMock = new Mock<IAIService>();
        string? usedPrompt = null;
        aiServiceMock
            .Setup(x => x.GenerateResponseAsync(It.IsAny<string>()))
            .Callback<string>(prompt => usedPrompt = prompt)
            .ReturnsAsync("Esta es la respuesta del asistente.");

        var sut = new ChatService(chatRepositoryMock.Object, aiServiceMock.Object);

        var result = await sut.ProcessMessageAsync(request);

        result.Response.Should().Be("Esta es la respuesta del asistente.");
        usedPrompt.Should().NotBeNullOrWhiteSpace();
        usedPrompt.Should().Contain("DATOS ACTUALES");
        usedPrompt.Should().Contain("HISTORIAL");
        usedPrompt.Should().Contain("Usuario: Mensaje previo");
        usedPrompt.Should().Contain("Pregunta actual: ¿Cómo está el aire hoy?");

        chatRepositoryMock.Verify(x => x.GetHistoryAsync(request.SessionId), Times.Once);
        aiServiceMock.Verify(x => x.GenerateResponseAsync(It.IsAny<string>()), Times.Once);
        chatRepositoryMock.Verify(
            x => x.SaveAsync(It.Is<ChatMessage>(m =>
                m.SessionId == request.SessionId &&
                m.UserMessage == request.Message &&
                m.AssistantResponse == "Esta es la respuesta del asistente.")),
            Times.Once);
    }
}
