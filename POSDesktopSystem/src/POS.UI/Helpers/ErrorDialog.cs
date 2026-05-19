namespace POS.UI.Helpers;

public static class ErrorDialog
{
    public static void ShowError(IWin32Window? owner, string message, string title = "Error")
    {
        MessageBox.Show(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static void ShowWarning(IWin32Window? owner, string message, string title = "Warning")
    {
        MessageBox.Show(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    public static void ShowInformation(IWin32Window? owner, string message, string title = "Information")
    {
        MessageBox.Show(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    public static void ShowValidationError(IWin32Window? owner, string message, string title = "Validation Error")
    {
        MessageBox.Show(owner, message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    public static bool Confirm(IWin32Window? owner, string message, string title = "Confirm")
    {
        return MessageBox.Show(owner, message, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    }
}
