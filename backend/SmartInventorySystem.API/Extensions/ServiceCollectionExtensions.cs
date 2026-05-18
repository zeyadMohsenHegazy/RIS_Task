using Microsoft.OpenApi.Models;

namespace SmartInventorySystem.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Smart Inventory System API",
                Version = "v1",
                Description = "Smart Inventory Management System — REST API"
            });
        });

        // JWT authentication — configure when auth is implemented
        // services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)...

        return services;
    }
}
