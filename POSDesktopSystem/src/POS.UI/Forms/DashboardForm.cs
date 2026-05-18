using Microsoft.Extensions.DependencyInjection;
using POS.Application.DTOs;
using POS.Application.Interfaces;
using POS.Application.Services;
using POS.UI.Helpers;
using POS.UI.Navigation;
using POS.UI.Views;

namespace POS.UI.Forms;

public partial class DashboardForm : Form
{
    private readonly ISessionContext _sessionContext;
    private readonly IAuthService _authService;
    private readonly ISystemStatusProvider _systemStatusProvider;
    private readonly IInvoiceSyncService? _invoiceSyncService;
    private readonly IServiceProvider _serviceProvider;
    private readonly DashboardNavigator _navigator;

    private Button? _activeNavButton;

    public DashboardForm(
        ISessionContext sessionContext,
        IAuthService authService,
        ISystemStatusProvider systemStatusProvider,
        IServiceProvider serviceProvider,
        IInvoiceSyncService? invoiceSyncService = null)
    {
        _sessionContext = sessionContext ?? throw new ArgumentNullException(nameof(sessionContext));
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _systemStatusProvider = systemStatusProvider ?? throw new ArgumentNullException(nameof(systemStatusProvider));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _invoiceSyncService = invoiceSyncService;

        InitializeComponent();
        _navigator = new DashboardNavigator(pnlContent);

        ConfigureForCurrentUser();
        ConfigureSystemStatus();
        WireNavigationEvents();
        NavigateTo(DashboardSection.Pos);
    }

    private void ConfigureForCurrentUser()
    {
        var user = _sessionContext.CurrentUser
            ?? throw new InvalidOperationException("Dashboard requires an authenticated user.");

        lblAppTitle.Text = "POS Desktop";
        lblWelcome.Text = user.Username;
        lblRole.Text = user.RoleDisplayName;

        btnNavPos.Visible = RolePermissions.CanProcessSales(user);
        btnNavProducts.Visible = RolePermissions.CanManageProducts(user);
        btnNavInvoices.Visible = RolePermissions.CanViewInvoiceHistory(user);

        pnlManagerBadge.Visible = user.IsManager;
        pnlCashierBadge.Visible = user.IsCashier;
        btnSyncNow.Visible = user.IsManager && _systemStatusProvider.GetStatus().RequiresSync;
    }

    private void ConfigureSystemStatus()
    {
        lblSystemStatus.Text = _systemStatusProvider.GetStatus().StatusText;
    }

    private void WireNavigationEvents()
    {
        btnNavPos.Click += (_, _) => NavigateTo(DashboardSection.Pos);
        btnNavProducts.Click += (_, _) => NavigateTo(DashboardSection.Products);
        btnNavInvoices.Click += (_, _) => NavigateTo(DashboardSection.InvoiceHistory);
        btnSyncNow.Click += async (_, _) => await SyncNowAsync();

        btnNavPos.MouseEnter += NavButton_MouseEnter;
        btnNavProducts.MouseEnter += NavButton_MouseEnter;
        btnNavInvoices.MouseEnter += NavButton_MouseEnter;

        btnNavPos.MouseLeave += NavButton_MouseLeave;
        btnNavProducts.MouseLeave += NavButton_MouseLeave;
        btnNavInvoices.MouseLeave += NavButton_MouseLeave;
    }

    private async Task SyncNowAsync()
    {
        if (_invoiceSyncService is null)
        {
            return;
        }

        btnSyncNow.Enabled = false;
        try
        {
            var result = await _invoiceSyncService.SyncPendingInvoicesAsync();
            lblSystemStatus.Text = result.Message;
            AppLogging.LogAction("Manual sync", result.Message);
            ErrorDialog.ShowInformation(this, result.Message, "Sync");
        }
        catch (Exception ex)
        {
            AppLogging.LogError("Manual sync failed.", ex);
            ErrorDialog.ShowError(this, Exceptions.ExceptionMapper.GetUserMessage(ex, "syncing invoices"), "Sync");
        }
        finally
        {
            btnSyncNow.Enabled = true;
            ConfigureSystemStatus();
        }
    }

    private void NavigateTo(DashboardSection section)
    {
        var user = _sessionContext.CurrentUser;
        if (user is null)
        {
            return;
        }

        if (!CanAccessSection(section, user))
        {
            return;
        }

        UserControl view = section switch
        {
            DashboardSection.Pos => _serviceProvider.GetRequiredService<PosView>(),
            DashboardSection.Products => _serviceProvider.GetRequiredService<ProductsView>(),
            DashboardSection.InvoiceHistory => _serviceProvider.GetRequiredService<InvoiceHistoryView>(),
            _ => throw new ArgumentOutOfRangeException(nameof(section))
        };

        _navigator.ShowView(section, view);
        HighlightNavButton(section);
        lblContentTitle.Text = GetSectionTitle(section);
    }

    private static bool CanAccessSection(DashboardSection section, AuthenticatedUserDto user) =>
        section switch
        {
            DashboardSection.Pos => RolePermissions.CanProcessSales(user),
            DashboardSection.Products => RolePermissions.CanManageProducts(user),
            DashboardSection.InvoiceHistory => RolePermissions.CanViewInvoiceHistory(user),
            _ => false
        };

    private static string GetSectionTitle(DashboardSection section) =>
        section switch
        {
            DashboardSection.Pos => "Point of Sale",
            DashboardSection.Products => "Products Management",
            DashboardSection.InvoiceHistory => "Invoice History",
            _ => string.Empty
        };

    private void HighlightNavButton(DashboardSection section)
    {
        ResetNavButton(btnNavPos);
        ResetNavButton(btnNavProducts);
        ResetNavButton(btnNavInvoices);

        _activeNavButton = section switch
        {
            DashboardSection.Pos => btnNavPos,
            DashboardSection.Products => btnNavProducts,
            DashboardSection.InvoiceHistory => btnNavInvoices,
            _ => null
        };

        if (_activeNavButton is not null)
        {
            _activeNavButton.BackColor = UiTheme.PrimaryLight;
            _activeNavButton.Font = UiTheme.SectionFont;
        }
    }

    private void ResetNavButton(Button button)
    {
        button.BackColor = UiTheme.PrimaryDark;
        button.Font = UiTheme.BodyFont;
    }

    private void NavButton_MouseEnter(object? sender, EventArgs e)
    {
        if (sender is Button button && button != _activeNavButton)
        {
            button.BackColor = UiTheme.PrimaryHover;
        }
    }

    private void NavButton_MouseLeave(object? sender, EventArgs e)
    {
        if (sender is Button button && button != _activeNavButton)
        {
            button.BackColor = UiTheme.PrimaryDark;
        }
    }

    private void btnLogout_Click(object sender, EventArgs e)
    {
        _navigator.Clear();
        _authService.Logout();
        DialogResult = DialogResult.Retry;
        Close();
    }

    private void DashboardForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        _navigator.Clear();

        if (DialogResult != DialogResult.Retry && _sessionContext.IsAuthenticated)
        {
            _authService.Logout();
        }
    }
}
