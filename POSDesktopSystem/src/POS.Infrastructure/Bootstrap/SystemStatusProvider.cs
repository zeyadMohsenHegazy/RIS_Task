using POS.Application.DTOs;
using POS.Application.Interfaces;
using POS.Infrastructure.Data;

namespace POS.Infrastructure.Bootstrap;

public sealed class SystemStatusProvider : ISystemStatusProvider
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly SyncOptions _syncOptions;

    public SystemStatusProvider(
        IDatabaseConnectionFactory connectionFactory,
        SyncOptions syncOptions)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _syncOptions = syncOptions ?? throw new ArgumentNullException(nameof(syncOptions));
    }

    public SystemStatusDto GetStatus()
    {
        var provider = _connectionFactory.Provider == DatabaseProviderType.Sqlite ? "SQLite (Offline)" : "SQL Server";
        var syncActive = _syncOptions.Enabled && _connectionFactory.RequiresSync;

        var statusText = syncActive
            ? $"{provider} · Background sync enabled"
            : _connectionFactory.RequiresSync
                ? $"{provider} · Sync available (disabled)"
                : provider;

        return new SystemStatusDto
        {
            DatabaseProvider = provider,
            SyncEnabled = syncActive,
            RequiresSync = _connectionFactory.RequiresSync,
            StatusText = statusText
        };
    }
}
