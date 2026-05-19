namespace POS.Application.DTOs;

public sealed class ServiceResult
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;

    public static ServiceResult Success(string message = "Operation completed successfully.") =>
        new() { IsSuccess = true, Message = message };

    public static ServiceResult Failure(string message) =>
        new() { IsSuccess = false, Message = message };
}

public sealed class ServiceResult<T>
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static ServiceResult<T> Success(T data, string message = "Operation completed successfully.") =>
        new() { IsSuccess = true, Message = message, Data = data };

    public static ServiceResult<T> Failure(string message) =>
        new() { IsSuccess = false, Message = message };
}
