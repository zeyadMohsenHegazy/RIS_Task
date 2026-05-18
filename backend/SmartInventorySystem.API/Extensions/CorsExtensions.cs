using SmartInventorySystem.API.Settings;

namespace SmartInventorySystem.API.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var corsSettings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>()
            ?? new CorsSettings();

        var origins = corsSettings.AllowedOrigins
            .Where(o => !string.IsNullOrWhiteSpace(o))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        services.AddCors(options =>
        {
            options.AddPolicy(CorsSettings.PolicyName, policy =>
            {
                policy.AllowAnyHeader().AllowAnyMethod();

                if (origins.Length > 0)
                {
                    policy.WithOrigins(origins).AllowCredentials();
                }
                else
                {
                    policy.AllowAnyOrigin();
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app)
    {
        return app.UseCors(CorsSettings.PolicyName);
    }
}
