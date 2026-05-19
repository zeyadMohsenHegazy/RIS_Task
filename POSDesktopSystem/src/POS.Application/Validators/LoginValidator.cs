namespace POS.Application.Validators;

public sealed class LoginValidator
{
    public ValidationResult Validate(string username, string password)
    {
        return ValidationHelper.Combine(
            ValidationHelper.Required(username, "Username"),
            ValidationHelper.Required(password, "Password"),
            ValidationHelper.MaxLength(username, 100, "Username"),
            ValidationHelper.MaxLength(password, 128, "Password"));
    }
}
