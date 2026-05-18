using System.Net;
using System.Text.Json;
using FluentValidation;
using SmartInventorySystem.API.Models;

namespace SmartInventorySystem.API.Middleware;

public class ExceptionMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}",
                context.TraceIdentifier);

            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message, errors) = MapException(exception);

        var response = new ErrorResponse
        {
            StatusCode = (int)statusCode,
            Message = message,
            Details = GetDetails(exception, statusCode),
            Errors = errors,
            TraceId = context.TraceIdentifier
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private static (HttpStatusCode StatusCode, string Message, IDictionary<string, string[]>? Errors)
        MapException(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                "Validation failed.",
                validationException.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray())),

            UnauthorizedAccessException unauthorizedException => (
                HttpStatusCode.Unauthorized,
                string.IsNullOrWhiteSpace(unauthorizedException.Message)
                    ? "Unauthorized."
                    : unauthorizedException.Message,
                null),

            KeyNotFoundException keyNotFoundException => (
                HttpStatusCode.NotFound,
                string.IsNullOrWhiteSpace(keyNotFoundException.Message)
                    ? "Resource not found."
                    : keyNotFoundException.Message,
                null),

            InvalidOperationException invalidOperationException when IsNotFoundMessage(invalidOperationException.Message) => (
                HttpStatusCode.NotFound,
                invalidOperationException.Message,
                null),

            InvalidOperationException invalidOperationException => (
                HttpStatusCode.BadRequest,
                invalidOperationException.Message,
                null),

            ArgumentException argumentException => (
                HttpStatusCode.BadRequest,
                argumentException.Message,
                null),

            _ => (
                HttpStatusCode.InternalServerError,
                "An unexpected error occurred.",
                null)
        };
    }

    private string? GetDetails(Exception exception, HttpStatusCode statusCode)
    {
        if (statusCode != HttpStatusCode.InternalServerError)
        {
            return null;
        }

        return _environment.IsDevelopment()
            ? exception.ToString()
            : null;
    }

    private static bool IsNotFoundMessage(string? message)
    {
        return !string.IsNullOrWhiteSpace(message)
            && message.Contains("not found", StringComparison.OrdinalIgnoreCase);
    }
}
