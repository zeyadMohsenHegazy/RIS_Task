using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SmartInventorySystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        _ = configuration;

        // Persistence (DbContext) — register when entities and DbContext exist
        // Repositories — register when interfaces and implementations exist
        // Authentication — register when JWT/auth is implemented
        services.AddMemoryCache();
        // Seed — invoke from host when seed data is implemented

        return services;
    }
}
