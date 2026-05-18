using Microsoft.Extensions.DependencyInjection;
using POS.Application.Interfaces;
using POS.Infrastructure.Data;

namespace POS.Infrastructure.Sync;

public sealed class BackgroundSyncService : IDisposable
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly SyncOptions _options;
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly IAppLogger _logger;
    private Timer? _timer;
    private int _isRunning;

    public BackgroundSyncService(
        IServiceScopeFactory scopeFactory,
        SyncOptions options,
        IDatabaseConnectionFactory connectionFactory,
        IAppLogger logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Start()
    {
        if (!_options.Enabled || !_connectionFactory.RequiresSync)
        {
            return;
        }

        var interval = TimeSpan.FromSeconds(Math.Max(15, _options.IntervalSeconds));
        _timer = new Timer(_ => _ = RunSyncAsync(), null, TimeSpan.FromSeconds(5), interval);
        _logger.LogInformation($"Background sync started (interval: {interval.TotalSeconds}s).");
    }

    private async Task RunSyncAsync()
    {
        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) == 1)
        {
            return;
        }

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var syncService = scope.ServiceProvider.GetRequiredService<IInvoiceSyncService>();
            var result = await syncService.SyncPendingInvoicesAsync();

            if (result.SyncedCount > 0 || result.FailedCount > 0)
            {
                _logger.LogAction("Background sync completed", result.Message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Background sync failed.", ex);
        }
        finally
        {
            Interlocked.Exchange(ref _isRunning, 0);
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
