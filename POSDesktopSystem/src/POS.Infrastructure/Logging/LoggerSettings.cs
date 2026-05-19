namespace POS.Infrastructure.Logging;

public sealed class LoggerSettings
{
    public const string SectionName = "Logging";

    public string LogDirectory { get; set; } = "Logs";
    public string FileNamePrefix { get; set; } = "pos";
}
