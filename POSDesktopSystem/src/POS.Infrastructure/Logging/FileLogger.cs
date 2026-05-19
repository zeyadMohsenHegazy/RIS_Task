using POS.Application.Interfaces;

namespace POS.Infrastructure.Logging;

public sealed class FileLogger : IAppLogger, IDisposable
{
    private readonly string _logDirectory;
    private readonly string _fileNamePrefix;
    private readonly object _writeLock = new();
    private bool _disposed;

    public FileLogger()
        : this(Path.Combine(AppContext.BaseDirectory, "Logs"), "pos")
    {
    }

    public FileLogger(string logDirectory, string fileNamePrefix = "pos")
    {
        if (string.IsNullOrWhiteSpace(logDirectory))
        {
            throw new ArgumentException("Log directory cannot be empty.", nameof(logDirectory));
        }

        _logDirectory = logDirectory;
        _fileNamePrefix = string.IsNullOrWhiteSpace(fileNamePrefix) ? "pos" : fileNamePrefix;
        Directory.CreateDirectory(_logDirectory);
    }

    public void LogInformation(string message) => Write("INFO", message);

    public void LogWarning(string message) => Write("WARN", message);

    public void LogError(string message, Exception? exception = null)
    {
        var fullMessage = exception is null
            ? message
            : $"{message}{Environment.NewLine}{exception}";

        Write("ERROR", fullMessage);
    }

    public void LogAction(string action, string? details = null)
    {
        var message = string.IsNullOrWhiteSpace(details)
            ? action
            : $"{action} | {details}";

        Write("ACTION", message);
    }

    private void Write(string level, string message)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";
        var filePath = Path.Combine(_logDirectory, $"{_fileNamePrefix}-{DateTime.Now:yyyy-MM-dd}.log");

        lock (_writeLock)
        {
            File.AppendAllText(filePath, entry + Environment.NewLine);
        }
    }

    public void Dispose()
    {
        _disposed = true;
    }
}
