using System.Drawing.Printing;
using POS.Application.DTOs;

namespace POS.UI.Printing;

public sealed class ReceiptPrintRenderer
{
    private readonly InvoiceDetailDto _invoice;
    private readonly ReceiptFormat _format;
    private int _itemIndex;

    public ReceiptPrintRenderer(InvoiceDetailDto invoice, ReceiptFormat format)
    {
        _invoice = invoice ?? throw new ArgumentNullException(nameof(invoice));
        _format = format;
    }

    public void OnPrintPage(object? sender, PrintPageEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e.Graphics);

        var layout = CreateLayout(e.MarginBounds, _format);
        var graphics = e.Graphics;
        var y = layout.Top;

        if (_itemIndex == 0)
        {
            y = DrawHeader(graphics, layout, y);
            y = DrawInvoiceInfo(graphics, layout, y);
            y = DrawItemsHeader(graphics, layout, y);
        }

        y = DrawItems(graphics, layout, y, out var hasMoreItems);
        if (!hasMoreItems)
        {
            y = DrawTotals(graphics, layout, y);
            DrawFooter(graphics, layout, y);
        }

        e.HasMorePages = hasMoreItems;
    }

    private int DrawItems(Graphics graphics, ReceiptLayout layout, int y, out bool hasMoreItems)
    {
        hasMoreItems = false;

        while (_itemIndex < _invoice.Items.Count)
        {
            var item = _invoice.Items[_itemIndex];
            var blockHeight = MeasureItemBlockHeight(graphics, layout, item);

            if (y + blockHeight > layout.Bottom)
            {
                hasMoreItems = true;
                return y;
            }

            y = DrawItem(graphics, layout, y, item);
            _itemIndex++;
        }

        return y;
    }

    private static int DrawHeader(Graphics graphics, ReceiptLayout layout, int y)
    {
        using var titleFont = new Font("Segoe UI", layout.TitleSize, FontStyle.Bold);
        using var brush = new SolidBrush(Color.Black);

        var title = "POS Desktop System";
        var titleSize = graphics.MeasureString(title, titleFont, layout.ContentWidth);
        graphics.DrawString(title, titleFont, brush, layout.Left + (layout.ContentWidth - titleSize.Width) / 2, y);
        y += (int)Math.Ceiling(titleSize.Height) + 4;

        using var subtitleFont = new Font("Segoe UI", layout.BodySize, FontStyle.Regular);
        var subtitle = "Sales Receipt";
        var subtitleSize = graphics.MeasureString(subtitle, subtitleFont, layout.ContentWidth);
        graphics.DrawString(subtitle, subtitleFont, brush, layout.Left + (layout.ContentWidth - subtitleSize.Width) / 2, y);
        y += (int)Math.Ceiling(subtitleSize.Height) + layout.LineSpacing;

        return DrawSeparator(graphics, layout, y);
    }

    private int DrawInvoiceInfo(Graphics graphics, ReceiptLayout layout, int y)
    {
        using var font = new Font("Segoe UI", layout.BodySize, FontStyle.Regular);
        using var brush = new SolidBrush(Color.Black);

        y = DrawLabelValue(graphics, layout, font, brush, y, "Invoice #", _invoice.Id.ToString());
        y = DrawLabelValue(graphics, layout, font, brush, y, "Date", _invoice.Date.ToString("g"));
        y = DrawLabelValue(graphics, layout, font, brush, y, "Cashier", _invoice.CashierName);
        y = DrawLabelValue(graphics, layout, font, brush, y, "Payment", _invoice.PaymentMethod);

        return DrawSeparator(graphics, layout, y);
    }

    private static int DrawItemsHeader(Graphics graphics, ReceiptLayout layout, int y)
    {
        using var font = new Font("Segoe UI", layout.BodySize, FontStyle.Bold);
        using var brush = new SolidBrush(Color.Black);

        if (layout.IsThermal)
        {
            graphics.DrawString("Items", font, brush, layout.Left, y);
            y += layout.LineHeight + layout.LineSpacing;
            return y;
        }

        graphics.DrawString("Product", font, brush, layout.ProductColumn, y);
        graphics.DrawString("Qty", font, brush, layout.QuantityColumn, y);
        graphics.DrawString("Unit", font, brush, layout.UnitPriceColumn, y);
        graphics.DrawString("Subtotal", font, brush, layout.SubtotalColumn, y);
        y += layout.LineHeight + layout.LineSpacing;

        return DrawSeparator(graphics, layout, y);
    }

    private int DrawItem(Graphics graphics, ReceiptLayout layout, int y, InvoiceItemDetailDto item)
    {
        using var nameFont = new Font("Segoe UI", layout.BodySize, FontStyle.Regular);
        using var detailFont = new Font("Segoe UI", Math.Max(6f, layout.BodySize - 1f), FontStyle.Regular);
        using var brush = new SolidBrush(Color.Black);

        if (layout.IsThermal)
        {
            var nameHeight = DrawWrappedText(graphics, item.ProductName, nameFont, brush, layout.Left, y, layout.ContentWidth);
            y += nameHeight + 2;

            var detail = $"{item.Quantity} x {item.UnitPrice:C2}";
            graphics.DrawString(detail, detailFont, brush, layout.Left, y);
            var subtotalText = item.SubTotal.ToString("C2");
            var subtotalSize = graphics.MeasureString(subtotalText, detailFont);
            graphics.DrawString(subtotalText, detailFont, brush, layout.Right - subtotalSize.Width, y);

            return y + layout.LineHeight + layout.LineSpacing;
        }

        var productHeight = DrawWrappedText(
            graphics,
            item.ProductName,
            nameFont,
            brush,
            layout.ProductColumn,
            y,
            layout.ProductWidth);

        graphics.DrawString(item.Quantity.ToString(), nameFont, brush, layout.QuantityColumn, y);
        graphics.DrawString(item.UnitPrice.ToString("C2"), nameFont, brush, layout.UnitPriceColumn, y);
        graphics.DrawString(item.SubTotal.ToString("C2"), nameFont, brush, layout.SubtotalColumn, y);

        return y + Math.Max(productHeight, layout.LineHeight) + layout.LineSpacing;
    }

    private int DrawTotals(Graphics graphics, ReceiptLayout layout, int y)
    {
        y = DrawSeparator(graphics, layout, y);

        using var font = new Font("Segoe UI", layout.BodySize, FontStyle.Regular);
        using var totalFont = new Font("Segoe UI", layout.BodySize + 2, FontStyle.Bold);
        using var brush = new SolidBrush(Color.Black);

        y = DrawAmountRow(graphics, layout, font, brush, y, "Subtotal", _invoice.TotalAmount);
        y = DrawAmountRow(graphics, layout, font, brush, y, "Discount", _invoice.Discount);
        y = DrawAmountRow(graphics, layout, font, brush, y, "Tax", _invoice.Tax);
        y = DrawAmountRow(graphics, layout, totalFont, brush, y, "Final Total", _invoice.FinalAmount);

        return y + layout.LineSpacing;
    }

    private static void DrawFooter(Graphics graphics, ReceiptLayout layout, int y)
    {
        y = DrawSeparator(graphics, layout, y);

        using var font = new Font("Segoe UI", layout.BodySize, FontStyle.Italic);
        using var brush = new SolidBrush(Color.Black);
        var message = "Thank you for your purchase!";
        var size = graphics.MeasureString(message, font, layout.ContentWidth);
        graphics.DrawString(message, font, brush, layout.Left + (layout.ContentWidth - size.Width) / 2, y);
    }

    private static int DrawLabelValue(
        Graphics graphics,
        ReceiptLayout layout,
        Font font,
        Brush brush,
        int y,
        string label,
        string value)
    {
        var safeValue = string.IsNullOrWhiteSpace(value) ? "—" : value;
        graphics.DrawString($"{label}:", font, brush, layout.Left, y);
        var valueSize = graphics.MeasureString(safeValue, font);
        var valueX = Math.Max(layout.Left, layout.Right - valueSize.Width);
        graphics.DrawString(safeValue, font, brush, valueX, y);
        return y + layout.LineHeight;
    }

    private static int DrawAmountRow(
        Graphics graphics,
        ReceiptLayout layout,
        Font font,
        Brush brush,
        int y,
        string label,
        decimal amount)
    {
        graphics.DrawString(label, font, brush, layout.Left, y);
        var value = amount.ToString("C2");
        var valueSize = graphics.MeasureString(value, font);
        graphics.DrawString(value, font, brush, layout.Right - valueSize.Width, y);
        return y + layout.LineHeight;
    }

    private static int DrawSeparator(Graphics graphics, ReceiptLayout layout, int y)
    {
        using var pen = new Pen(Color.Black, 1f);
        graphics.DrawLine(pen, layout.Left, y, layout.Right, y);
        return y + layout.LineSpacing;
    }

    private static int DrawWrappedText(
        Graphics graphics,
        string text,
        Font font,
        Brush brush,
        float x,
        float y,
        float maxWidth)
    {
        var rect = new RectangleF(x, y, maxWidth, 1000);
        graphics.DrawString(text, font, brush, rect);
        var size = graphics.MeasureString(text, font, new SizeF(maxWidth, 1000), StringFormat.GenericDefault);
        return (int)Math.Ceiling(size.Height);
    }

    private static int MeasureItemBlockHeight(Graphics graphics, ReceiptLayout layout, InvoiceItemDetailDto item)
    {
        using var nameFont = new Font("Segoe UI", layout.BodySize, FontStyle.Regular);

        if (layout.IsThermal)
        {
            var nameHeight = (int)Math.Ceiling(graphics.MeasureString(
                item.ProductName,
                nameFont,
                new SizeF(layout.ContentWidth, 1000),
                StringFormat.GenericDefault).Height);
            return nameHeight + layout.LineHeight + layout.LineSpacing + 2;
        }

        var productHeight = (int)Math.Ceiling(graphics.MeasureString(
            item.ProductName,
            nameFont,
            new SizeF(layout.ProductWidth, 1000),
            StringFormat.GenericDefault).Height);
        return Math.Max(productHeight, layout.LineHeight) + layout.LineSpacing;
    }

    private static ReceiptLayout CreateLayout(Rectangle marginBounds, ReceiptFormat format)
    {
        return format == ReceiptFormat.Thermal
            ? ReceiptLayout.ForThermal(marginBounds)
            : ReceiptLayout.ForA4(marginBounds);
    }

    private sealed class ReceiptLayout
    {
        public int Top { get; init; }
        public int Bottom { get; init; }
        public int Left { get; init; }
        public int Right { get; init; }
        public int ContentWidth { get; init; }
        public int LineHeight { get; init; }
        public int LineSpacing { get; init; }
        public float TitleSize { get; init; }
        public float BodySize { get; init; }
        public bool IsThermal { get; init; }
        public float ProductColumn { get; init; }
        public float QuantityColumn { get; init; }
        public float UnitPriceColumn { get; init; }
        public float SubtotalColumn { get; init; }
        public float ProductWidth { get; init; }

        public static ReceiptLayout ForA4(Rectangle bounds) =>
            new()
            {
                Top = bounds.Top,
                Bottom = Math.Max(bounds.Top + 200, bounds.Bottom),
                Left = bounds.Left,
                Right = Math.Max(bounds.Left + 200, bounds.Right),
                ContentWidth = Math.Max(200, bounds.Width),
                LineHeight = 18,
                LineSpacing = 8,
                TitleSize = 16f,
                BodySize = 10f,
                IsThermal = false,
                ProductColumn = bounds.Left,
                QuantityColumn = bounds.Left + bounds.Width * 0.55f,
                UnitPriceColumn = bounds.Left + bounds.Width * 0.67f,
                SubtotalColumn = bounds.Left + bounds.Width * 0.80f,
                ProductWidth = bounds.Width * 0.50f
            };

        public static ReceiptLayout ForThermal(Rectangle bounds) =>
            new()
            {
                Top = bounds.Top,
                Bottom = Math.Max(bounds.Top + 200, bounds.Bottom),
                Left = bounds.Left,
                Right = Math.Max(bounds.Left + 120, bounds.Right),
                ContentWidth = Math.Max(120, bounds.Width),
                LineHeight = 16,
                LineSpacing = 6,
                TitleSize = 11f,
                BodySize = 8.5f,
                IsThermal = true
            };
    }
}
