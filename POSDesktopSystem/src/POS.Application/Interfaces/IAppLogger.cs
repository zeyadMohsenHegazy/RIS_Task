namespace POS.Application.Interfaces;

public interface IAppLogger
{
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message, Exception? exception = null);
    void LogAction(string action, string? details = null);
}
