using System.Drawing.Printing;
using POS.Application.DTOs;

namespace POS.UI.Printing;

public static class ReceiptPrinter
{
    public static void ShowPreview(
        InvoiceDetailDto invoice,
        ReceiptFormat format,
        IWin32Window? owner = null)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        using var document = CreatePrintDocument(invoice, format);
        using var previewDialog = new PrintPreviewDialog
        {
            Document = document,
            Width = format == ReceiptFormat.Thermal ? 420 : 900,
            Height = 700,
            StartPosition = FormStartPosition.CenterParent,
            Text = $"Receipt Preview - Invoice #{invoice.Id}"
        };

        previewDialog.ShowDialog(owner);
    }

    public static void Print(
        InvoiceDetailDto invoice,
        ReceiptFormat format,
        IWin32Window? owner = null)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        using var document = CreatePrintDocument(invoice, format);
        using var printDialog = new PrintDialog
        {
            Document = document,
            UseEXDialog = true,
            AllowSomePages = false
        };

        if (printDialog.ShowDialog(owner) != DialogResult.OK)
        {
            return;
        }

        document.Print();
    }

    public static PrintDocument CreatePrintDocument(InvoiceDetailDto invoice, ReceiptFormat format)
    {
        ArgumentNullException.ThrowIfNull(invoice);

        var renderer = new ReceiptPrintRenderer(invoice, format);
        var document = new PrintDocument
        {
            DocumentName = $"Invoice_{invoice.Id}_Receipt"
        };

        ConfigurePageSettings(document, format);
        document.PrintPage += renderer.OnPrintPage;
        return document;
    }

    private static void ConfigurePageSettings(PrintDocument document, ReceiptFormat format)
    {
        if (format == ReceiptFormat.Thermal)
        {
            document.DefaultPageSettings.PaperSize = new PaperSize("Thermal Receipt", 315, 1169);
            document.DefaultPageSettings.Landscape = false;
            document.DefaultPageSettings.Margins = new Margins(15, 15, 20, 20);
            return;
        }

        document.DefaultPageSettings.PaperSize = new PaperSize("A4", 827, 1169);
        document.DefaultPageSettings.Landscape = false;
        document.DefaultPageSettings.Margins = new Margins(60, 60, 60, 60);
    }
}
