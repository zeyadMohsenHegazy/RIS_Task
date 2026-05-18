using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS.Infrastructure.Data;
using POS.Infrastructure.DependencyInjection;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();
services.AddInfrastructure(configuration);

await using var provider = services.BuildServiceProvider();
await using var scope = provider.CreateAsyncScope();

var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
await initializer.InitializeAsync();

Console.WriteLine("POS database migration and seed completed successfully.");
