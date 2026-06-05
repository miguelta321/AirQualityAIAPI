using AirQualityIAAPI.Api.Controllers;
using AirQualityIAAPI.Application.DTOs;
using AirQualityIAAPI.Application.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AirQualityAIAPI.UnitTests.Api.Controllers;

public class ChatControllerTests
{
    [Fact]
    public async Task Chat_ShouldReturnOkWithServiceResponse()
    {
        var request = new ChatRequestDto
        {
            SessionId = "session-1",
            Message = "Hola",
            AirData = new AirQualityDataDto()
        };

        var expected = new ChatResponseDto { Response = "Respuesta IA" };

        var chatServiceMock = new Mock<IChatService>();
        chatServiceMock
            .Setup(x => x.ProcessMessageAsync(request))
            .ReturnsAsync(expected);

        var sut = new ChatController(chatServiceMock.Object);

        var result = await sut.Chat(request);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.Value.Should().BeEquivalentTo(expected);
    }
}
