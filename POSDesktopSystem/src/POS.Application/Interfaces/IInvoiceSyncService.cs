namespace POS.Application.Interfaces;

public interface IInvoiceSyncService
{
    Task<SyncResult> SyncPendingInvoicesAsync(CancellationToken cancellationToken = default);
}

public sealed class SyncResult
{
    public int SyncedCount { get; init; }

    public int FailedCount { get; init; }

    public string Message { get; init; } = string.Empty;
}
