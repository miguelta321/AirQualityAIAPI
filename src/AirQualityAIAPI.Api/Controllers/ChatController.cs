using AirQualityIAAPI.Application.DTOs;
using AirQualityIAAPI.Application.Interfaces;
using AirQualityIAAPI.Domain.Entities;
using AirQualityIAAPI.Domain.Interfaces;
using AirQualityIAAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AirQualityIAAPI.Api.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    [HttpPost]
    public async Task<IActionResult> Chat([FromBody] ChatRequestDto request)
    {
        var response = await _chatService.ProcessMessageAsync(request);

        return Ok(response);
    }
}