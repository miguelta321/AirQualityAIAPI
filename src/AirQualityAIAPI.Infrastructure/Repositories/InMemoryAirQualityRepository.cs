using System.Collections.Concurrent;
using AirQualityAIAPI.Domain.Entities;
using AirQualityAIAPI.Domain.Interfaces;

namespace AirQualityAIAPI.Infrastructure.Repositories;

public class InMemoryAirQualityRepository : IAirQualityRepository
{
    private readonly ConcurrentDictionary<Guid, AirQualityReading> _store = new();

    public Task<IEnumerable<AirQualityReading>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<AirQualityReading>>(_store.Values.OrderByDescending(r => r.Timestamp));
    }

    public Task<AirQualityReading?> GetByIdAsync(Guid id)
    {
        _store.TryGetValue(id, out var reading);
        return Task.FromResult(reading);
    }

    public Task<IEnumerable<AirQualityReading>> GetByLocationAsync(string location)
    {
        var results = _store.Values
            .Where(r => r.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(r => r.Timestamp);
        return Task.FromResult<IEnumerable<AirQualityReading>>(results);
    }

    public Task AddAsync(AirQualityReading reading)
    {
        _store[reading.Id] = reading;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(AirQualityReading reading)
    {
        _store[reading.Id] = reading;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id)
    {
        _store.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
