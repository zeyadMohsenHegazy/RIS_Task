using POS.Application.DTOs;
using POS.UI.Exceptions;
using POS.UI.Export;
using POS.UI.Helpers;
using POS.UI.Printing;

namespace POS.UI.Forms;

public partial class InvoiceDetailForm : Form
{
    private readonly InvoiceDetailDto _invoice;

    public InvoiceDetailForm(InvoiceDetailDto invoice, bool showSyncInfo = false)
    {
        _invoice = invoice ?? throw new ArgumentNullException(nameof(invoice));
        InitializeComponent();
        BindInvoice(invoice, showSyncInfo);
    }

    private void BindInvoice(InvoiceDetailDto invoice, bool showSyncInfo)
    {
        Text = $"Invoice #{invoice.Id}";
        lblInvoiceTitle.Text = $"Invoice #{invoice.Id}";
        lblDateValue.Text = invoice.Date.ToString("f");
        lblCreatedAtValue.Text = invoice.CreatedAt.ToString("f");
        lblCashierValue.Text = invoice.CashierName;
        lblPaymentValue.Text = invoice.PaymentMethod;
        lblSubtotalValue.Text = invoice.TotalAmount.ToString("C2");
        lblDiscountValue.Text = invoice.Discount.ToString("C2");
        lblTaxValue.Text = invoice.Tax.ToString("C2");
        lblFinalValue.Text = invoice.FinalAmount.ToString("C2");

        if (showSyncInfo && invoice.SyncStatus != "—")
        {
            var syncDetails = invoice.SyncedAt.HasValue
                ? $"{invoice.SyncStatus} at {invoice.SyncedAt.Value:g}"
                : invoice.SyncStatus;

            if (!string.IsNullOrWhiteSpace(invoice.SyncError))
            {
                syncDetails += $" — {invoice.SyncError}";
            }

            lblFinalValue.Text += $"   |   Sync: {syncDetails}";
        }

        cmbReceiptFormat.Items.Clear();
        cmbReceiptFormat.Items.Add("A4");
        cmbReceiptFormat.Items.Add("Thermal (80mm)");
        cmbReceiptFormat.SelectedIndex = 0;

        DataGridHelper.ConfigureReadOnlyGrid(dgvItems);
        dgvItems.DataSource = invoice.Items.ToList();
    }

    private ReceiptFormat GetSelectedFormat() =>
        cmbReceiptFormat.SelectedIndex == 1 ? ReceiptFormat.Thermal : ReceiptFormat.A4;

    private void btnPrintPreview_Click(object sender, EventArgs e)
    {
        ReceiptPrinter.ShowPreview(_invoice, GetSelectedFormat(), this);
    }

    private void btnPrint_Click(object sender, EventArgs e)
    {
        ReceiptPrinter.Print(_invoice, GetSelectedFormat(), this);
    }

    private void btnExportPdf_Click(object sender, EventArgs e)
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "PDF files (*.pdf)|*.pdf",
            FileName = $"Invoice-{_invoice.Id}.pdf",
            Title = "Export Invoice to PDF"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        try
        {
            InvoicePdfExporter.Export(_invoice, dialog.FileName);
            ErrorDialog.ShowInformation(this, $"Invoice exported to:{Environment.NewLine}{dialog.FileName}", "Export PDF");
        }
        catch (Exception ex)
        {
            AppLogging.LogError("Error exporting invoice to PDF.", ex);
            ErrorDialog.ShowError(this, ExceptionMapper.GetUserMessage(ex, "exporting invoice to PDF"), "Export PDF");
        }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
        Close();
    }
}
