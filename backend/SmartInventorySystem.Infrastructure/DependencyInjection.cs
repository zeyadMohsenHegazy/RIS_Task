using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartInventorySystem.Infrastructure.Persistence;

namespace SmartInventorySystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        // Repositories — register when interfaces and implementations exist
        // Authentication — register when JWT/auth is implemented
        services.AddMemoryCache();
        // Seed — invoke from host when seed data is implemented

        return services;
    }
}
