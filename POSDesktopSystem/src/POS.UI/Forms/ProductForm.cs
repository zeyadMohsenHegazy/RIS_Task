using POS.Application.DTOs;
using POS.Application.Validators;
using POS.UI.Helpers;

namespace POS.UI.Forms;

public partial class ProductForm : Form
{
    private readonly ProductValidator _productValidator;
    private readonly ProductUpsertDto? _existingProduct;
    private readonly ErrorProvider _errorProvider;

    public ProductForm(ProductValidator productValidator, ProductDto? product = null)
    {
        _productValidator = productValidator ?? throw new ArgumentNullException(nameof(productValidator));
        _errorProvider = new ErrorProvider { ContainerControl = this };

        _existingProduct = product is null
            ? null
            : new ProductUpsertDto
            {
                Name = product.Name,
                Barcode = product.Barcode,
                Price = product.Price,
                StockQuantity = product.StockQuantity
            };

        InitializeComponent();
        ConfigureForm(product);
    }

    public ProductForm(ProductValidator productValidator, ProductUpsertDto input, string? errorMessage = null)
    {
        _productValidator = productValidator ?? throw new ArgumentNullException(nameof(productValidator));
        _errorProvider = new ErrorProvider { ContainerControl = this };
        ArgumentNullException.ThrowIfNull(input);
        _existingProduct = input;

        InitializeComponent();
        Text = "Add Product";
        lblFormTitle.Text = Text;
        txtName.Text = input.Name;
        txtBarcode.Text = input.Barcode;
        numPrice.Value = ClampNumeric(input.Price, numPrice.Minimum, numPrice.Maximum);
        numStock.Value = ClampNumeric(input.StockQuantity, numStock.Minimum, numStock.Maximum);

        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            ShowError(errorMessage);
        }
    }

    public ProductForm(
        ProductValidator productValidator,
        ProductDto product,
        ProductUpsertDto input,
        string? errorMessage = null)
    {
        _productValidator = productValidator ?? throw new ArgumentNullException(nameof(productValidator));
        _errorProvider = new ErrorProvider { ContainerControl = this };
        ArgumentNullException.ThrowIfNull(product);
        ArgumentNullException.ThrowIfNull(input);
        _existingProduct = input;

        InitializeComponent();
        Text = "Edit Product";
        lblFormTitle.Text = Text;
        txtName.Text = input.Name;
        txtBarcode.Text = input.Barcode;
        numPrice.Value = ClampNumeric(input.Price, numPrice.Minimum, numPrice.Maximum);
        numStock.Value = ClampNumeric(input.StockQuantity, numStock.Minimum, numStock.Maximum);

        if (!string.IsNullOrWhiteSpace(errorMessage))
        {
            ShowError(errorMessage);
        }
    }

    public ProductUpsertDto ProductInput => new()
    {
        Name = txtName.Text.Trim(),
        Barcode = txtBarcode.Text.Trim(),
        Price = numPrice.Value,
        StockQuantity = (int)numStock.Value
    };

    public bool IsEditMode => _existingProduct is not null;

    private void ConfigureForm(ProductDto? product)
    {
        Text = product is null ? "Add Product" : "Edit Product";
        lblFormTitle.Text = Text;

        if (product is not null)
        {
            txtName.Text = product.Name;
            txtBarcode.Text = product.Barcode;
            numPrice.Value = ClampNumeric(product.Price, numPrice.Minimum, numPrice.Maximum);
            numStock.Value = ClampNumeric(product.StockQuantity, numStock.Minimum, numStock.Maximum);
        }
    }

    private static decimal ClampNumeric(decimal value, decimal min, decimal max) =>
        value < min ? min : value > max ? max : value;

    private static decimal ClampNumeric(int value, decimal min, decimal max)
    {
        var decimalValue = (decimal)value;
        return decimalValue < min ? min : decimalValue > max ? max : decimalValue;
    }

    public void ShowError(string message)
    {
        FormValidationHelper.ShowInlineError(lblErrorMessage, message);
    }

    public void ClearError()
    {
        FormValidationHelper.HideInlineError(lblErrorMessage);
        ClearFieldErrors();
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        ClearError();

        var input = ProductInput;
        var validation = _productValidator.Validate(input);
        if (!validation.IsValid)
        {
            ApplyFieldValidation(input);
            ShowError(validation.ErrorMessage);
            FocusFirstInvalidField(input);
            return;
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void ApplyFieldValidation(ProductUpsertDto input)
    {
        FormValidationHelper.SetFieldError(
            _errorProvider,
            txtName,
            string.IsNullOrWhiteSpace(input.Name) ? "Product name is required." : string.Empty);

        FormValidationHelper.SetFieldError(
            _errorProvider,
            txtBarcode,
            string.IsNullOrWhiteSpace(input.Barcode) ? "Barcode is required." : string.Empty);
    }

    private void ClearFieldErrors()
    {
        FormValidationHelper.ClearFieldErrors(_errorProvider, txtName, txtBarcode, numPrice, numStock);
    }

    private void FocusFirstInvalidField(ProductUpsertDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            txtName.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(input.Barcode))
        {
            txtBarcode.Focus();
            return;
        }

        txtName.Focus();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _errorProvider.Dispose();
            components?.Dispose();
        }

        base.Dispose(disposing);
    }
}
