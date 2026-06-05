using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AirQualityIAAPI.Domain.Entities;

public class ChatMessage
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public string UserMessage { get; set; } = string.Empty;
    public string AssistantResponse { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}