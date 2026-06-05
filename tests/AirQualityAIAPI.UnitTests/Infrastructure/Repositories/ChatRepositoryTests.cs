using AirQualityIAAPI.Domain.Entities;
using AirQualityIAAPI.Infrastructure.Repositories;
using FluentAssertions;
using Moq;
using MongoDB.Driver;

namespace AirQualityAIAPI.UnitTests.Infrastructure.Repositories;

public class ChatRepositoryTests
{
    [Fact]
    public async Task GetHistoryAsync_ShouldLimitToTenAndReturnAscendingByDate()
    {
        var itemsFromMongo = new List<ChatMessage>
        {
            new() { SessionId = "s-1", UserMessage = "u3", AssistantResponse = "a3", CreatedAt = new DateTime(2025, 1, 1, 10, 3, 0, DateTimeKind.Utc) },
            new() { SessionId = "s-1", UserMessage = "u1", AssistantResponse = "a1", CreatedAt = new DateTime(2025, 1, 1, 10, 1, 0, DateTimeKind.Utc) },
            new() { SessionId = "s-1", UserMessage = "u2", AssistantResponse = "a2", CreatedAt = new DateTime(2025, 1, 1, 10, 2, 0, DateTimeKind.Utc) }
        };

        var cursorMock = new Mock<IAsyncCursor<ChatMessage>>();
        cursorMock
            .SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
            .Returns(true)
            .Returns(false);
        cursorMock
            .SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        cursorMock
            .SetupGet(x => x.Current)
            .Returns(itemsFromMongo);

        var collectionMock = new Mock<IMongoCollection<ChatMessage>>();
        collectionMock
            .Setup(x => x.FindAsync(
                It.IsAny<FilterDefinition<ChatMessage>>(),
                It.Is<FindOptions<ChatMessage, ChatMessage>>(o => o.Limit == 10),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(cursorMock.Object);

        var sut = new ChatRepository(collectionMock.Object);

        var result = await sut.GetHistoryAsync("s-1");

        result.Select(x => x.UserMessage).Should().ContainInOrder("u1", "u2", "u3");
        collectionMock.Verify(
            x => x.FindAsync(
                It.IsAny<FilterDefinition<ChatMessage>>(),
                It.Is<FindOptions<ChatMessage, ChatMessage>>(o => o.Limit == 10),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveAsync_ShouldInsertMessage()
    {
        var message = new ChatMessage
        {
            SessionId = "s-1",
            UserMessage = "hola",
            AssistantResponse = "respuesta",
            CreatedAt = DateTime.UtcNow
        };

        var collectionMock = new Mock<IMongoCollection<ChatMessage>>();
        var sut = new ChatRepository(collectionMock.Object);

        await sut.SaveAsync(message);

        collectionMock.Verify(
            x => x.InsertOneAsync(
                message,
                It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}
