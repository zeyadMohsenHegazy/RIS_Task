namespace SmartInventorySystem.API.Settings;

public class CorsSettings
{
    public const string SectionName = "Cors";
    public const string PolicyName = "Frontend";

    public string[] AllowedOrigins { get; set; } =
    [
        "http://localhost:8081",
        "http://localhost:5173",
        "http://localhost:4200"
    ];
}
