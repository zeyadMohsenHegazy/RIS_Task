using Microsoft.Extensions.DependencyInjection;
using SmartInventorySystem.Application.Interfaces;

namespace SmartInventorySystem.Infrastructure.Repositories;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IWarehouseRepository, WarehouseRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();

        return services;
    }
}
