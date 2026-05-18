using POS.UI.Exceptions;

namespace POS.UI.Helpers;

public static class FormValidationHelper
{
    public static void ShowInlineError(Label errorLabel, string message)
    {
        ArgumentNullException.ThrowIfNull(errorLabel);
        errorLabel.Text = message;
        errorLabel.Visible = true;
    }

    public static void HideInlineError(Label errorLabel)
    {
        ArgumentNullException.ThrowIfNull(errorLabel);
        errorLabel.Text = string.Empty;
        errorLabel.Visible = false;
    }

    public static void SetFieldError(ErrorProvider errorProvider, Control control, string? message)
    {
        ArgumentNullException.ThrowIfNull(errorProvider);
        ArgumentNullException.ThrowIfNull(control);
        errorProvider.SetError(control, message ?? string.Empty);
    }

    public static void ClearFieldErrors(ErrorProvider errorProvider, params Control[] controls)
    {
        ArgumentNullException.ThrowIfNull(errorProvider);

        foreach (var control in controls)
        {
            errorProvider.SetError(control, string.Empty);
        }
    }
}
