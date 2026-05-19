using POS.UI.Exceptions;

namespace POS.UI.Helpers;

public static class GlobalExceptionHandler
{
    private static bool _isRegistered;

    public static void Register()
    {
        if (_isRegistered)
        {
            return;
        }

        System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        System.Windows.Forms.Application.ThreadException += OnThreadException;
        AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;
        _isRegistered = true;
    }

    private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
        AppLogging.LogError("Unhandled UI thread exception.", e.Exception);
        ErrorDialog.ShowError(null, ExceptionMapper.GetUserMessage(e.Exception), "Application Error");
    }

    private static void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is Exception exception)
        {
            AppLogging.LogError("Unhandled domain exception.", exception);
            ErrorDialog.ShowError(null, ExceptionMapper.GetUserMessage(exception), "Critical Error");
        }
    }
}
