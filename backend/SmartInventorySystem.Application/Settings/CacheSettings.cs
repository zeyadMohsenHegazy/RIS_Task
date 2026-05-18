namespace SmartInventorySystem.Application.Settings;

public class CacheSettings
{
    public const string SectionName = "Cache";

    public int DefaultExpirationMinutes { get; set; } = 5;
}
