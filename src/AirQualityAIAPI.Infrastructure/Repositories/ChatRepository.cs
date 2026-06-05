using AirQualityIAAPI.Domain.Entities;
using AirQualityIAAPI.Domain.Interfaces;
using AirQualityIAAPI.Infrastructure.MongoDB;
using MongoDB.Driver;

namespace AirQualityIAAPI.Infrastructure.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly IMongoCollection<ChatMessage> _collection;

    public ChatRepository(
        IMongoCollection<ChatMessage> collection)
    {
        _collection = collection;
    }

    public ChatRepository(MongoDBContext context)
    {
        _collection =
            context.GetCollection<ChatMessage>(
                "chat_history");
    }

    public async Task<List<ChatMessage>> GetHistoryAsync(
        string sessionId)
    {
        var filter = Builders<ChatMessage>
            .Filter
            .Eq(x => x.SessionId, sessionId);

        var cursor = await _collection.FindAsync(
            filter,
            new FindOptions<ChatMessage>
            {
                Sort = Builders<ChatMessage>
                    .Sort
                    .Descending(x => x.CreatedAt),
                Limit = 10
            });

        var history = await cursor
            .ToListAsync();

        return history
            .OrderBy(x => x.CreatedAt)
            .ToList();
    }

    public async Task SaveAsync(
        ChatMessage message)
    {
        await _collection.InsertOneAsync(message);
    }
}