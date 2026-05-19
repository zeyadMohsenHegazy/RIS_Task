using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS.Application.DependencyInjection;
using POS.Application.Interfaces;
using POS.Infrastructure.Bootstrap;
using POS.Infrastructure.DependencyInjection;
using POS.UI.Forms;
using POS.UI.Helpers;
using POS.UI.Session;
using POS.UI.Views;
namespace POS.UI.DependencyInjection;

public static class CompositionRoot
{
    private static IServiceProvider? _serviceProvider;

    public static IServiceProvider ServiceProvider =>
        _serviceProvider ?? throw new InvalidOperationException("Composition root has not been built. Call Build() first.");

    public static IServiceProvider Build()
    {
        if (_serviceProvider is not null)
        {
            return _serviceProvider;
        }

        var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
            ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Production";

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddSingleton<ISessionContext, SessionContext>();
        services.AddInfrastructure(configuration);
        services.AddApplication();
        services.AddTransient<LoginForm>();
        services.AddTransient<DashboardForm>();
        services.AddTransient<PosView>();
        services.AddTransient<ProductsView>();
        services.AddTransient<InvoiceHistoryView>();

        _serviceProvider = services.BuildServiceProvider();
        AppLogging.Initialize(_serviceProvider.GetRequiredService<IAppLogger>());
        return _serviceProvider;
    }

    public static T GetRequiredService<T>() where T : notnull
    {
        return ServiceProvider.GetRequiredService<T>();
    }
}
