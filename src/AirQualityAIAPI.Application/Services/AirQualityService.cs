using AirQualityAIAPI.Application.DTOs;
using AirQualityAIAPI.Application.Interfaces;
using AirQualityAIAPI.Domain.Entities;
using AirQualityAIAPI.Domain.Interfaces;

namespace AirQualityAIAPI.Application.Services;

public class AirQualityService : IAirQualityService
{
    private readonly IAirQualityRepository _repository;

    public AirQualityService(IAirQualityRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AirQualityReadingDto>> GetAllReadingsAsync()
    {
        var readings = await _repository.GetAllAsync();
        return readings.Select(MapToDto);
    }

    public async Task<AirQualityReadingDto?> GetReadingByIdAsync(Guid id)
    {
        var reading = await _repository.GetByIdAsync(id);
        return reading is null ? null : MapToDto(reading);
    }

    public async Task<IEnumerable<AirQualityReadingDto>> GetReadingsByLocationAsync(string location)
    {
        var readings = await _repository.GetByLocationAsync(location);
        return readings.Select(MapToDto);
    }

    public async Task<AirQualityReadingDto> CreateReadingAsync(CreateAirQualityReadingRequest request)
    {
        var reading = new AirQualityReading
        {
            Location = request.Location,
            AirQualityIndex = request.AirQualityIndex,
            Category = DetermineCategory(request.AirQualityIndex),
            Pm25 = request.Pm25,
            Pm10 = request.Pm10,
            Co2 = request.Co2,
            No2 = request.No2
        };

        await _repository.AddAsync(reading);
        return MapToDto(reading);
    }

    public async Task<AirQualityReadingDto?> UpdateReadingAsync(Guid id, UpdateAirQualityReadingRequest request)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return null;

        existing.Location = request.Location;
        existing.AirQualityIndex = request.AirQualityIndex;
        existing.Category = DetermineCategory(request.AirQualityIndex);
        existing.Pm25 = request.Pm25;
        existing.Pm10 = request.Pm10;
        existing.Co2 = request.Co2;
        existing.No2 = request.No2;

        await _repository.UpdateAsync(existing);
        return MapToDto(existing);
    }

    public async Task<bool> DeleteReadingAsync(Guid id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing is null)
            return false;

        await _repository.DeleteAsync(id);
        return true;
    }

    private static AirQualityReadingDto MapToDto(AirQualityReading reading) => new()
    {
        Id = reading.Id,
        Location = reading.Location,
        Timestamp = reading.Timestamp,
        AirQualityIndex = reading.AirQualityIndex,
        Category = reading.Category,
        Pm25 = reading.Pm25,
        Pm10 = reading.Pm10,
        Co2 = reading.Co2,
        No2 = reading.No2
    };

    private static string DetermineCategory(double aqi) => aqi switch
    {
        <= 50 => "Good",
        <= 100 => "Moderate",
        <= 150 => "Unhealthy for Sensitive Groups",
        <= 200 => "Unhealthy",
        <= 300 => "Very Unhealthy",
        _ => "Hazardous"
    };
}
