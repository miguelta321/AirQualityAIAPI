namespace AirQualityIAAPI.Application.Interfaces;

public interface IAIService
{
    Task<string> GenerateResponseAsync(string prompt);
}