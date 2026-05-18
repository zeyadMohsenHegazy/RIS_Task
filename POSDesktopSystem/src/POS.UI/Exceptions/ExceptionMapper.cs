using Microsoft.EntityFrameworkCore;
using POS.Application.Exceptions;

namespace POS.UI.Exceptions;

public static class ExceptionMapper
{
    public static string GetUserMessage(Exception exception, string? operationContext = null)
    {
        ArgumentNullException.ThrowIfNull(exception);

        return exception switch
        {
            UserFriendlyException userFriendly => userFriendly.Message,
            DbUpdateException => "A database error occurred. Please verify your connection and try again.",
            TimeoutException => "The operation timed out. Please try again.",
            InvalidOperationException invalidOperation when !string.IsNullOrWhiteSpace(invalidOperation.Message)
                => invalidOperation.Message,
            _ => string.IsNullOrWhiteSpace(operationContext)
                ? "An unexpected error occurred. Please try again."
                : $"Unable to complete {operationContext}. Please try again."
        };
    }
}
