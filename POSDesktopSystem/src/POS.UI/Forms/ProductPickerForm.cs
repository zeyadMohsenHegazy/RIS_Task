using POS.Application.DTOs;
using POS.UI.Helpers;

namespace POS.UI.Forms;

public sealed class ProductPickerForm : Form
{
    private readonly IReadOnlyList<ProductDto> _products;
    private readonly ListBox _listBox;
    private readonly Button _btnSelect;
    private readonly Button _btnCancel;

    public ProductDto? SelectedProduct { get; private set; }

    public ProductPickerForm(IReadOnlyList<ProductDto> products)
    {
        _products = products ?? throw new ArgumentNullException(nameof(products));

        Text = "Select Product";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterParent;
        MaximizeBox = false;
        MinimizeBox = false;
        ClientSize = new Size(520, 360);
        BackColor = UiTheme.Surface;

        var lblHint = new Label
        {
            Text = "Multiple products matched. Select one:",
            AutoSize = true,
            Location = new Point(20, 16),
            Font = UiTheme.SectionFont
        };

        _listBox = new ListBox
        {
            Location = new Point(20, 44),
            Size = new Size(480, 250),
            Font = UiTheme.BodyFont,
            IntegralHeight = false
        };

        foreach (var product in _products)
        {
            _listBox.Items.Add($"{product.Barcode} | {product.Name} | {product.Price:C2} | Stock: {product.StockQuantity}");
        }

        if (_listBox.Items.Count > 0)
        {
            _listBox.SelectedIndex = 0;
        }

        _btnSelect = new Button
        {
            Text = "Select",
            Location = new Point(300, 310),
            Size = new Size(90, 32)
        };
        UiTheme.StylePrimaryButton(_btnSelect);

        _btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(410, 310),
            Size = new Size(90, 32),
            DialogResult = DialogResult.Cancel
        };

        _btnSelect.Click += (_, _) => SelectCurrentItem();
        _listBox.DoubleClick += (_, _) => SelectCurrentItem();

        AcceptButton = _btnSelect;
        CancelButton = _btnCancel;

        Controls.Add(lblHint);
        Controls.Add(_listBox);
        Controls.Add(_btnSelect);
        Controls.Add(_btnCancel);
    }

    private void SelectCurrentItem()
    {
        if (_listBox.SelectedIndex < 0 || _listBox.SelectedIndex >= _products.Count)
        {
            return;
        }

        SelectedProduct = _products[_listBox.SelectedIndex];
        DialogResult = DialogResult.OK;
        Close();
    }
}
