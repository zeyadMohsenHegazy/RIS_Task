using POS.Application.DTOs;
using POS.Application.Interfaces;
using POS.UI.Exceptions;
using POS.UI.Forms;
using POS.UI.Helpers;

namespace POS.UI.Views;

public partial class InvoiceHistoryView : UserControl
{
    private readonly IInvoiceService _invoiceService;
    private readonly IDatabaseInfo _databaseInfo;
    private readonly SearchDebouncer _searchDebouncer;

    public InvoiceHistoryView(IInvoiceService invoiceService, IDatabaseInfo databaseInfo)
    {
        _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
        _databaseInfo = databaseInfo ?? throw new ArgumentNullException(nameof(databaseInfo));
        _searchDebouncer = new SearchDebouncer(RefreshInvoicesAsync);

        InitializeComponent();
        ConfigureGrid();
        WireEvents();
    }

    private void ConfigureGrid()
    {
        DataGridHelper.ConfigureReadOnlyGrid(dgvInvoices);
        dgvInvoices.Columns.Clear();
        DataGridHelper.AddTextColumn(dgvInvoices, nameof(InvoiceSummaryDto.Id), "Invoice #", 60);
        DataGridHelper.AddTextColumn(dgvInvoices, nameof(InvoiceSummaryDto.Date), "Date", 120, "g");
        DataGridHelper.AddTextColumn(dgvInvoices, nameof(InvoiceSummaryDto.CashierName), "Cashier", 90);
        DataGridHelper.AddTextColumn(dgvInvoices, nameof(InvoiceSummaryDto.ItemCount), "Items", 50);
        DataGridHelper.AddTextColumn(dgvInvoices, nameof(InvoiceSummaryDto.FinalAmount), "Final", 80, "C2");
        DataGridHelper.AddTextColumn(dgvInvoices, nameof(InvoiceSummaryDto.PaymentMethod), "Payment", 70);

        if (_databaseInfo.RequiresSync)
        {
            DataGridHelper.AddTextColumn(dgvInvoices, nameof(InvoiceSummaryDto.SyncStatus), "Sync", 70);
        }
    }

    private void WireEvents()
    {
        Load += async (_, _) => await RefreshInvoicesAsync();
        txtSearch.TextChanged += (_, _) => _searchDebouncer.Schedule();
        btnRefresh.Click += async (_, _) => await RefreshInvoicesAsync();
        btnViewDetails.Click += async (_, _) => await ViewSelectedInvoiceAsync();
        dgvInvoices.CellDoubleClick += async (_, _) => await ViewSelectedInvoiceAsync();
        dgvInvoices.SelectionChanged += (_, _) => UpdateActionButtons();
    }

    private InvoiceSummaryDto? GetSelectedInvoice() =>
        dgvInvoices.CurrentRow?.DataBoundItem as InvoiceSummaryDto;

    private void UpdateActionButtons()
    {
        btnViewDetails.Enabled = GetSelectedInvoice() is not null;
    }

    private async Task RefreshInvoicesAsync()
    {
        SetBusyState(true);
        try
        {
            var invoices = await _invoiceService.SearchInvoicesAsync(txtSearch.Text);
            dgvInvoices.DataSource = invoices.ToList();
            lblStatus.Text = $"{invoices.Count} invoice(s) found";
            UpdateActionButtons();
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Unable to load invoices.";
            AppLogging.LogError("Error loading invoices.", ex);
            ErrorDialog.ShowError(FindForm(), ExceptionMapper.GetUserMessage(ex, "loading invoices"), "Invoice History");
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private async Task ViewSelectedInvoiceAsync()
    {
        var selected = GetSelectedInvoice();
        if (selected is null)
        {
            return;
        }

        SetBusyState(true);
        try
        {
            var detail = await _invoiceService.GetInvoiceDetailAsync(selected.Id);
            if (detail is null)
            {
                ErrorDialog.ShowWarning(FindForm(), "Invoice could not be found.", "Invoice History");
                await RefreshInvoicesAsync();
                return;
            }

            using var form = new InvoiceDetailForm(detail, _databaseInfo.RequiresSync);
            form.ShowDialog(FindForm());
        }
        catch (Exception ex)
        {
            AppLogging.LogError("Error loading invoice details.", ex);
            ErrorDialog.ShowError(FindForm(), ExceptionMapper.GetUserMessage(ex, "loading invoice details"), "Invoice History");
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private void SetBusyState(bool isBusy)
    {
        UseWaitCursor = isBusy;
        btnRefresh.Enabled = !isBusy;
        btnViewDetails.Enabled = !isBusy && GetSelectedInvoice() is not null;
        txtSearch.Enabled = !isBusy;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _searchDebouncer.Dispose();
            components?.Dispose();
        }

        base.Dispose(disposing);
    }
}
