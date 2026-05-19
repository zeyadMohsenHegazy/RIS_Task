using Microsoft.Extensions.DependencyInjection;
using POS.Application.Interfaces;
using POS.Application.Services;
using POS.Application.Validators;

namespace POS.Application.DependencyInjection;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<LoginValidator>();
        services.AddSingleton<ProductValidator>();
        services.AddSingleton<PosCheckoutValidator>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<PosCart>();
        services.AddScoped<IPosService, PosService>();

        return services;
    }
}
