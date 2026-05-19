namespace POS.Infrastructure.Data;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string Provider { get; set; } = "SqlServer";

    public string SqliteFileName { get; set; } = "pos-offline.db";

    public bool AutoMigrate { get; set; } = true;

    public bool SeedDefaultUsers { get; set; } = true;
}
