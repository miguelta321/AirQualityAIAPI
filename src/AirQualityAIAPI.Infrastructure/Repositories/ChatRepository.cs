using AirQualityIAAPI.Domain.Entities;
using AirQualityIAAPI.Domain.Interfaces;
using AirQualityIAAPI.Infrastructure.MongoDB;
using MongoDB.Driver;

namespace AirQualityIAAPI.Infrastructure.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly IMongoCollection<ChatMessage> _collection;

    public ChatRepository(MongoDBContext context)
    {
        _collection =
            context.GetCollection<ChatMessage>(
                "chat_history");
    }

    public async Task<List<ChatMessage>> GetHistoryAsync(
        string sessionId)
    {
        var history = await _collection
            .Find(x => x.SessionId == sessionId)
            .SortByDescending(x => x.CreatedAt)
            .Limit(10)
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