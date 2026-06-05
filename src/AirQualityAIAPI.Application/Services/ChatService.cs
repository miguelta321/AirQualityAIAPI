using AirQualityIAAPI.Application.DTOs;
using AirQualityIAAPI.Application.Interfaces;
using AirQualityIAAPI.Domain.Entities;
using AirQualityIAAPI.Domain.Interfaces;

namespace AirQualityIAAPI.Application.Services;

public class ChatService : IChatService
{
    private readonly IChatRepository _chatRepository;
    private readonly IAIService _aiService;

    public ChatService(
        IChatRepository chatRepository,
        IAIService aiService)
    {
        _chatRepository = chatRepository;
        _aiService = aiService;
    }

    public async Task<ChatResponseDto> ProcessMessageAsync(
        ChatRequestDto request)
    {
        var history = await _chatRepository.GetHistoryAsync(request.SessionId);

        var prompt =
            PromptBuilder.Build(request, history);

        var aiResponse =await _aiService.GenerateResponseAsync(prompt);

        await _chatRepository.SaveAsync(
            new ChatMessage
            {
                SessionId = request.SessionId,
                UserMessage = request.Message,
                AssistantResponse = aiResponse,
                CreatedAt = DateTime.UtcNow
            });

        return new ChatResponseDto
        {
            Response = aiResponse
        };
    }
}