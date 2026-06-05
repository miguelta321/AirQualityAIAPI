using AirQualityIAAPI.Application.Interfaces;
using AirQualityIAAPI.Application.Services;
using AirQualityIAAPI.Domain.Interfaces;
using AirQualityIAAPI.Infrastructure.AI;
using AirQualityIAAPI.Infrastructure.MongoDB;
using AirQualityIAAPI.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AirQualityIAAPI.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<MongoDBSettings>(
            configuration.GetSection("MongoDBSettings"));

        services.Configure<GeminiSettings>(
            configuration.GetSection("GeminiSettings"));

        services.AddSingleton<MongoDBContext>();
        services.AddScoped<IChatRepository, ChatRepository>();
        services.AddScoped<IChatService, ChatService>();

        services.AddHttpClient<IAIService, GeminiService>(
            client =>
            {
                client.BaseAddress =
                    new Uri("https://generativelanguage.googleapis.com/v1beta/");
            });

        return services;
    }
}