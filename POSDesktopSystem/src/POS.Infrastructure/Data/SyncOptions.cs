namespace POS.Infrastructure.Data;

public sealed class SyncOptions
{
    public const string SectionName = "Sync";

    public bool Enabled { get; set; }

    public int IntervalSeconds { get; set; } = 60;
}
