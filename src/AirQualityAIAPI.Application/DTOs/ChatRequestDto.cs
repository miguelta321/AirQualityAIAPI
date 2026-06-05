using System.ComponentModel.DataAnnotations;

namespace AirQualityIAAPI.Application.DTOs;

public class ChatRequestDto
{
    [Required]
    public string SessionId { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [Required]
    public AirQualityDataDto AirData { get; set; } = new();
}