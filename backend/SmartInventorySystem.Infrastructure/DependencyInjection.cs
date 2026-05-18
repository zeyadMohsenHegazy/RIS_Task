using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartInventorySystem.Infrastructure.Authentication;
using SmartInventorySystem.Infrastructure.Caching;
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
        services.AddAuthenticationServices(configuration);
        services.AddCachingServices(configuration);

        return services;
    }
}
