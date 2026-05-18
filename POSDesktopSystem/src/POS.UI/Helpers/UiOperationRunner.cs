using POS.UI.Exceptions;

namespace POS.UI.Helpers;

public static class UiOperationRunner
{
    public static async Task RunAsync(
        IWin32Window? owner,
        string operationContext,
        Func<Task> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            await action();
        }
        catch (Exception ex)
        {
            AppLogging.LogError($"Error during {operationContext}.", ex);
            ErrorDialog.ShowError(owner, ExceptionMapper.GetUserMessage(ex, operationContext));
        }
    }

    public static async Task<T?> RunAsync<T>(
        IWin32Window? owner,
        string operationContext,
        Func<Task<T>> action)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            return await action();
        }
        catch (Exception ex)
        {
            AppLogging.LogError($"Error during {operationContext}.", ex);
            ErrorDialog.ShowError(owner, ExceptionMapper.GetUserMessage(ex, operationContext));
            return null;
        }
    }

    public static void Run(
        IWin32Window? owner,
        string operationContext,
        Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        try
        {
            action();
        }
        catch (Exception ex)
        {
            AppLogging.LogError($"Error during {operationContext}.", ex);
            ErrorDialog.ShowError(owner, ExceptionMapper.GetUserMessage(ex, operationContext));
        }
    }
}
