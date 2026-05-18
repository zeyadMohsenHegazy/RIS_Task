using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartInventorySystem.Application.Settings;
using SmartInventorySystem.Domain.Constants;

namespace SmartInventorySystem.API.Extensions;

public static class AuthenticationExtensions
{
    public const string AdminOnlyPolicy = "AdminOnly";
    public const string EmployeeOnlyPolicy = "EmployeeOnly";
    public const string AdminOrEmployeePolicy = "AdminOrEmployee";

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT settings are not configured.");

        if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) || jwtSettings.SecretKey.Length < 32)
        {
            throw new InvalidOperationException("JWT SecretKey must be at least 32 characters.");
        }

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AdminOnlyPolicy, policy =>
                policy.RequireRole(Roles.Admin));

            options.AddPolicy(EmployeeOnlyPolicy, policy =>
                policy.RequireRole(Roles.Employee));

            options.AddPolicy(AdminOrEmployeePolicy, policy =>
                policy.RequireRole(Roles.Admin, Roles.Employee));
        });

        return services;
    }
}
