namespace POS.Application.Validators;

public static class ValidationHelper
{
    public static ValidationResult Required(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ValidationResult.Invalid($"{fieldName} is required.");
        }

        return ValidationResult.Valid();
    }

    public static ValidationResult MaxLength(string? value, int maxLength, string fieldName)
    {
        if (value is not null && value.Length > maxLength)
        {
            return ValidationResult.Invalid($"{fieldName} must not exceed {maxLength} characters.");
        }

        return ValidationResult.Valid();
    }

    public static ValidationResult NonNegative(decimal value, string fieldName)
    {
        if (value < 0)
        {
            return ValidationResult.Invalid($"{fieldName} cannot be negative.");
        }

        return ValidationResult.Valid();
    }

    public static ValidationResult GreaterThanZero(int value, string fieldName)
    {
        if (value <= 0)
        {
            return ValidationResult.Invalid($"{fieldName} must be greater than zero.");
        }

        return ValidationResult.Valid();
    }

    public static ValidationResult Combine(params ValidationResult[] results)
    {
        ArgumentNullException.ThrowIfNull(results);

        foreach (var result in results)
        {
            if (!result.IsValid)
            {
                return result;
            }
        }

        return ValidationResult.Valid();
    }
}
