namespace POS.Application.Validators;

public sealed class ValidationResult
{
    public bool IsValid { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;

    public static ValidationResult Valid() => new() { IsValid = true };

    public static ValidationResult Invalid(string message) =>
        new() { IsValid = false, ErrorMessage = message };
}
