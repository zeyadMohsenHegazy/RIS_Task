using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartInventorySystem.Infrastructure.Persistence;
using SmartInventorySystem.Infrastructure.Repositories;

namespace SmartInventorySystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddRepositories();

        // Authentication — register when JWT/auth is implemented
        services.AddMemoryCache();
        // Seed — invoke from host when seed data is implemented

        return services;
    }
}
