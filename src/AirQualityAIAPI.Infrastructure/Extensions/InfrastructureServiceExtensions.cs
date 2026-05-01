using AirQualityAIAPI.Application.Interfaces;
using AirQualityAIAPI.Application.Services;
using AirQualityAIAPI.Domain.Interfaces;
using AirQualityAIAPI.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace AirQualityAIAPI.Infrastructure.Extensions;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IAirQualityRepository, InMemoryAirQualityRepository>();
        services.AddSingleton<IAirQualityService, AirQualityService>();
        return services;
    }
}
