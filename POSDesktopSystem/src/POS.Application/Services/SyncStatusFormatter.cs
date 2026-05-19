using POS.Domain.Enums;

namespace POS.Application.Services;

public static class SyncStatusFormatter
{
    public static string GetLabel(SyncStatus status) =>
        status switch
        {
            SyncStatus.Pending => "Pending",
            SyncStatus.Synced => "Synced",
            SyncStatus.Failed => "Failed",
            SyncStatus.NotRequired => "—",
            _ => status.ToString()
        };
}
