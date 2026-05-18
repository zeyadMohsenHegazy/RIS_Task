using POS.Application.DTOs;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace POS.UI.Export;

public static class InvoicePdfExporter
{
    static InvoicePdfExporter()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public static void Export(InvoiceDetailDto invoice, string filePath)
    {
        ArgumentNullException.ThrowIfNull(invoice);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Column(column =>
                {
                    column.Item().Text("POS Desktop System").FontSize(18).SemiBold().FontColor(Colors.Blue.Darken2);
                    column.Item().Text($"Invoice #{invoice.Id}").FontSize(14).SemiBold();
                    column.Item().PaddingTop(8).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().PaddingVertical(16).Column(column =>
                {
                    column.Spacing(8);
                    column.Item().Row(row =>
                    {
                        row.RelativeItem().Column(info =>
                        {
                            info.Item().Text($"Date: {invoice.Date:f}");
                            info.Item().Text($"Created: {invoice.CreatedAt:f}");
                            info.Item().Text($"Cashier: {invoice.CashierName}");
                        });
                        row.RelativeItem().Column(info =>
                        {
                            info.Item().AlignRight().Text($"Payment: {invoice.PaymentMethod}");
                            info.Item().AlignRight().Text($"Items: {invoice.Items.Count}");
                        });
                    });

                    column.Item().PaddingTop(12).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.ConstantColumn(40);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Product").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Barcode").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("Qty").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("Unit").SemiBold();
                            header.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("Subtotal").SemiBold();
                        });

                        foreach (var item in invoice.Items)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(item.ProductName);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(item.Barcode);
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(item.Quantity.ToString());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(item.UnitPrice.ToString("C2"));
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(item.SubTotal.ToString("C2"));
                        }
                    });

                    column.Item().AlignRight().PaddingTop(16).Column(totals =>
                    {
                        totals.Item().Text($"Subtotal: {invoice.TotalAmount:C2}");
                        totals.Item().Text($"Discount: {invoice.Discount:C2}");
                        totals.Item().Text($"Tax: {invoice.Tax:C2}");
                        totals.Item().PaddingTop(4).Text($"Final Total: {invoice.FinalAmount:C2}").FontSize(12).SemiBold();
                    });
                });

                page.Footer().AlignCenter().Text(text =>
                {
                    text.Span("Generated ");
                    text.Span(DateTime.Now.ToString("f"));
                });
            });
        }).GeneratePdf(filePath);
    }
}
