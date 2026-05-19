using Microsoft.Extensions.DependencyInjection;
using POS.Application.Interfaces;
using POS.Infrastructure.Data;
using POS.Infrastructure.Sync;

namespace POS.Infrastructure.Bootstrap;

public sealed class ApplicationBootstrap : IApplicationBootstrap
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly BackgroundSyncService _backgroundSyncService;

    public ApplicationBootstrap(
        IServiceScopeFactory scopeFactory,
        BackgroundSyncService backgroundSyncService)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _backgroundSyncService = backgroundSyncService ?? throw new ArgumentNullException(nameof(backgroundSyncService));
    }

    public bool IsInitialized { get; private set; }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized)
        {
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
        await initializer.InitializeAsync(cancellationToken);
        IsInitialized = true;
    }

    public void StartBackgroundServices()
    {
        _backgroundSyncService.Start();
    }
}
