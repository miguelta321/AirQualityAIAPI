using AirQualityIAAPI.Domain.Entities;

namespace AirQualityIAAPI.Domain.Interfaces;

public interface IChatRepository
{
    Task<List<ChatMessage>> GetHistoryAsync(string sessionId);
    Task SaveAsync(ChatMessage message);
}