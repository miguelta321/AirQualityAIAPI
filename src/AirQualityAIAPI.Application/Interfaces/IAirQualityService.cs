using AirQualityAIAPI.Application.DTOs;

namespace AirQualityAIAPI.Application.Interfaces;

public interface IAirQualityService
{
    Task<IEnumerable<AirQualityReadingDto>> GetAllReadingsAsync();
    Task<AirQualityReadingDto?> GetReadingByIdAsync(Guid id);
    Task<IEnumerable<AirQualityReadingDto>> GetReadingsByLocationAsync(string location);
    Task<AirQualityReadingDto> CreateReadingAsync(CreateAirQualityReadingRequest request);
    Task<AirQualityReadingDto?> UpdateReadingAsync(Guid id, UpdateAirQualityReadingRequest request);
    Task<bool> DeleteReadingAsync(Guid id);
}
