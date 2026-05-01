using AirQualityAIAPI.Domain.Entities;

namespace AirQualityAIAPI.Domain.Interfaces;

public interface IAirQualityRepository
{
    Task<IEnumerable<AirQualityReading>> GetAllAsync();
    Task<AirQualityReading?> GetByIdAsync(Guid id);
    Task<IEnumerable<AirQualityReading>> GetByLocationAsync(string location);
    Task AddAsync(AirQualityReading reading);
    Task UpdateAsync(AirQualityReading reading);
    Task DeleteAsync(Guid id);
}
