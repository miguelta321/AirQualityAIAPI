using AirQualityIAAPI.Application.DTOs;

namespace AirQualityIAAPI.Application.Interfaces;

public interface IChatService
{
    Task<ChatResponseDto> ProcessMessageAsync(ChatRequestDto request);
}