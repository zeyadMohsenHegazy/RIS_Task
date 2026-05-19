using POS.Application.Interfaces;

namespace POS.UI.Helpers;

public static class AppLogging
{
    private static IAppLogger? _logger;

    public static void Initialize(IAppLogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogInformation("POS Desktop System started.");
    }

    public static void LogInformation(string message) => _logger?.LogInformation(message);

    public static void LogWarning(string message) => _logger?.LogWarning(message);

    public static void LogError(string message, Exception? exception = null) =>
        _logger?.LogError(message, exception);

    public static void LogAction(string action, string? details = null) =>
        _logger?.LogAction(action, details);
}
