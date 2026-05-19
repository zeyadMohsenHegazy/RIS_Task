using Microsoft.Extensions.DependencyInjection;
using POS.Application.DTOs;
using POS.Application.Interfaces;
using POS.Application.Validators;
using POS.UI.Helpers;

namespace POS.UI.Forms;

public partial class LoginForm : Form
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IApplicationBootstrap _applicationBootstrap;
    private readonly LoginValidator _loginValidator;

    public LoginForm(
        IServiceProvider serviceProvider,
        IApplicationBootstrap applicationBootstrap,
        LoginValidator loginValidator)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _applicationBootstrap = applicationBootstrap ?? throw new ArgumentNullException(nameof(applicationBootstrap));
        _loginValidator = loginValidator ?? throw new ArgumentNullException(nameof(loginValidator));
        InitializeComponent();
    }

    private async void LoginForm_Shown(object? sender, EventArgs e)
    {
        if (_applicationBootstrap.IsInitialized)
        {
            return;
        }

        SetBusyState(true);
        lblInitStatus.Visible = true;
        lblInitStatus.Text = "Initializing database...";

        try
        {
            await _applicationBootstrap.InitializeAsync();
            _applicationBootstrap.StartBackgroundServices();
            lblInitStatus.Text = "Ready";
        }
        catch (Exception ex)
        {
            AppLogging.LogError("Database initialization failed.", ex);
            lblInitStatus.ForeColor = UiTheme.Error;
            lblInitStatus.Text = "Database initialization failed. Check connection settings.";
            btnLogin.Enabled = false;
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        await AttemptLoginAsync();
    }

    private async void txtPassword_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            await AttemptLoginAsync();
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private async Task AttemptLoginAsync()
    {
        if (!_applicationBootstrap.IsInitialized)
        {
            FormValidationHelper.ShowInlineError(lblErrorMessage, "System is still initializing. Please wait.");
            return;
        }

        FormValidationHelper.HideInlineError(lblErrorMessage);

        var validation = _loginValidator.Validate(txtUsername.Text, txtPassword.Text);
        if (!validation.IsValid)
        {
            FormValidationHelper.ShowInlineError(lblErrorMessage, validation.ErrorMessage);
            FocusFirstInvalidField();
            return;
        }

        SetBusyState(true);

        try
        {
            await UiOperationRunner.RunAsync(this, "login", async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var authService = scope.ServiceProvider.GetRequiredService<IAuthService>();

                var result = await authService.LoginAsync(new LoginRequest
                {
                    Username = txtUsername.Text,
                    Password = txtPassword.Text
                });

                if (!result.IsSuccess)
                {
                    FormValidationHelper.ShowInlineError(lblErrorMessage, result.Message);
                    txtPassword.SelectAll();
                    txtPassword.Focus();
                    return;
                }

                DialogResult = DialogResult.OK;
                Close();
            });
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private void FocusFirstInvalidField()
    {
        if (string.IsNullOrWhiteSpace(txtUsername.Text))
        {
            txtUsername.Focus();
            return;
        }

        txtPassword.Focus();
    }

    private void SetBusyState(bool isBusy)
    {
        btnLogin.Enabled = !isBusy && _applicationBootstrap.IsInitialized;
        btnCancel.Enabled = !isBusy;
        txtUsername.Enabled = !isBusy;
        txtPassword.Enabled = !isBusy;
        Cursor = isBusy ? Cursors.WaitCursor : Cursors.Default;
    }
}
