using POS.Application.DTOs;
using POS.Application.Interfaces;
using POS.Application.Validators;
using POS.Domain.Enums;
using POS.UI.Exceptions;
using POS.UI.Forms;
using POS.UI.Helpers;
using POS.UI.Printing;

namespace POS.UI.Views;

public partial class PosView : UserControl
{
    private readonly IPosService _posService;
    private readonly IInvoiceService _invoiceService;
    private readonly PosCheckoutValidator _checkoutValidator;
    private ProductDto? _selectedProduct;

    public PosView(
        IPosService posService,
        IInvoiceService invoiceService,
        PosCheckoutValidator checkoutValidator)
    {
        _posService = posService ?? throw new ArgumentNullException(nameof(posService));
        _invoiceService = invoiceService ?? throw new ArgumentNullException(nameof(invoiceService));
        _checkoutValidator = checkoutValidator ?? throw new ArgumentNullException(nameof(checkoutValidator));
        InitializeComponent();
        ConfigureGrid();
        WireEvents();
        RefreshCart();
    }

    private void ConfigureGrid()
    {
        DataGridHelper.ConfigureReadOnlyGrid(dgvCart);
        dgvCart.Columns.Clear();
        DataGridHelper.AddTextColumn(dgvCart, nameof(PosCartItemDto.ProductName), "Product", 140);
        DataGridHelper.AddTextColumn(dgvCart, nameof(PosCartItemDto.Barcode), "Barcode", 90);
        DataGridHelper.AddTextColumn(dgvCart, nameof(PosCartItemDto.Quantity), "Qty", 50);
        DataGridHelper.AddTextColumn(dgvCart, nameof(PosCartItemDto.UnitPrice), "Unit Price", 80, "C2");
        DataGridHelper.AddTextColumn(dgvCart, nameof(PosCartItemDto.SubTotal), "Subtotal", 80, "C2");
    }

    private void WireEvents()
    {
        txtBarcode.KeyDown += async (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await LookupAndStageProductAsync();
            }
        };
        btnLookup.Click += async (_, _) => await LookupAndStageProductAsync();
        btnAddToCart.Click += (_, _) => AddStagedProductToCart();
        btnRemoveItem.Click += (_, _) => RemoveSelectedCartItem();
        btnClearCart.Click += (_, _) => ClearCart();
        btnUpdateQty.Click += async (_, _) => await UpdateSelectedCartQuantityAsync();
        numDiscount.ValueChanged += (_, _) => RefreshTotals();
        numTax.ValueChanged += (_, _) => RefreshTotals();
        btnPayCash.Click += async (_, _) => await CheckoutAsync(PaymentMethod.Cash);
        btnPayCard.Click += async (_, _) => await CheckoutAsync(PaymentMethod.Card);
        dgvCart.SelectionChanged += (_, _) => SyncSelectedCartItemQuantity();
    }

    private async Task LookupAndStageProductAsync()
    {
        lblLookupStatus.Text = string.Empty;

        var searchValidation = _checkoutValidator.ValidateSearchTerm(txtBarcode.Text);
        if (!searchValidation.IsValid)
        {
            ShowLookupError(searchValidation.ErrorMessage);
            return;
        }

        SetBusyState(true);

        try
        {
            var result = await _posService.SearchProductsAsync(txtBarcode.Text);
            if (!result.IsSuccess || result.Data is null)
            {
                _selectedProduct = null;
                lblProductInfo.Text = "No product selected";
                ShowLookupError(result.Message);
                return;
            }

            var lookup = result.Data;

            if (lookup.IsExactBarcodeMatch && lookup.Products.Count == 1)
            {
                _selectedProduct = lookup.Products[0];
                var productName = _selectedProduct.Name;
                var addResult = _posService.AddToCart(_selectedProduct, 1);
                if (!addResult.IsSuccess)
                {
                    ShowLookupError(addResult.Message);
                    return;
                }

                txtBarcode.Clear();
                _selectedProduct = null;
                lblProductInfo.Text = "No product selected";
                lblLookupStatus.ForeColor = UiTheme.Success;
                lblLookupStatus.Text = $"Added {productName} to cart (barcode scan).";
                RefreshCart();
                txtBarcode.Focus();
                return;
            }

            if (lookup.Products.Count > 1)
            {
                using var picker = new ProductPickerForm(lookup.Products);
                if (picker.ShowDialog(FindForm()) != DialogResult.OK || picker.SelectedProduct is null)
                {
                    return;
                }

                lookup = new ProductLookupResult
                {
                    IsExactBarcodeMatch = false,
                    Products = [picker.SelectedProduct]
                };
            }

            _selectedProduct = lookup.Products[0];
            lblProductInfo.Text = $"{_selectedProduct.Name} | {_selectedProduct.Barcode} | {_selectedProduct.Price:C2} | Stock: {_selectedProduct.StockQuantity}";
            lblLookupStatus.ForeColor = UiTheme.Success;
            lblLookupStatus.Text = "Product found. Set quantity and add to cart.";
            numQuantity.Focus();
        }
        catch (Exception ex)
        {
            ShowLookupError(ExceptionMapper.GetUserMessage(ex, "product lookup"));
            AppLogging.LogError("Error during product lookup.", ex);
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private void AddStagedProductToCart()
    {
        if (_selectedProduct is null)
        {
            ShowLookupError("Lookup a product before adding to cart.");
            return;
        }

        var quantityValidation = _checkoutValidator.ValidateQuantity((int)numQuantity.Value);
        if (!quantityValidation.IsValid)
        {
            ShowLookupError(quantityValidation.ErrorMessage);
            numQuantity.Focus();
            return;
        }

        var result = _posService.AddToCart(_selectedProduct, (int)numQuantity.Value);
        if (!result.IsSuccess)
        {
            ShowLookupError(result.Message);
            return;
        }

        txtBarcode.Clear();
        _selectedProduct = null;
        lblProductInfo.Text = "No product selected";
        lblLookupStatus.ForeColor = Color.FromArgb(46, 125, 50);
        lblLookupStatus.Text = result.Message;
        numQuantity.Value = 1;
        txtBarcode.Focus();
        RefreshCart();
    }

    private void RemoveSelectedCartItem()
    {
        var item = GetSelectedCartItem();
        if (item is null)
        {
            return;
        }

        var result = _posService.RemoveFromCart(item.ProductId);
        if (!result.IsSuccess)
        {
            ErrorDialog.ShowValidationError(FindForm(), result.Message, "POS");
            return;
        }

        RefreshCart();
    }

    private async Task UpdateSelectedCartQuantityAsync()
    {
        var item = GetSelectedCartItem();
        if (item is null)
        {
            ErrorDialog.ShowInformation(FindForm(), "Select a cart item to update quantity.", "POS");
            return;
        }

        var quantityValidation = _checkoutValidator.ValidateCartQuantity((int)numCartQuantity.Value);
        if (!quantityValidation.IsValid)
        {
            ErrorDialog.ShowValidationError(FindForm(), quantityValidation.ErrorMessage, "POS");
            return;
        }

        var result = await _posService.UpdateCartItemQuantityAsync(item.ProductId, (int)numCartQuantity.Value);
        if (!result.IsSuccess)
        {
            ErrorDialog.ShowValidationError(FindForm(), result.Message, "POS");
            return;
        }

        RefreshCart();
    }

    private void ClearCart()
    {
        if (_posService.CartItems.Count == 0)
        {
            return;
        }

        if (!ErrorDialog.Confirm(FindForm(), "Clear all items from the cart?", "Clear Cart"))
        {
            return;
        }

        _posService.ClearCart();
        RefreshCart();
    }

    private async Task CheckoutAsync(PaymentMethod paymentMethod)
    {
        if (_posService.CartItems.Count == 0)
        {
            ErrorDialog.ShowInformation(FindForm(), "Cart is empty.", "Checkout");
            return;
        }

        var discountValidation = _checkoutValidator.ValidateDiscount(numDiscount.Value);
        if (!discountValidation.IsValid)
        {
            ErrorDialog.ShowValidationError(FindForm(), discountValidation.ErrorMessage, "Checkout");
            return;
        }

        var taxValidation = _checkoutValidator.ValidateTax(numTax.Value);
        if (!taxValidation.IsValid)
        {
            ErrorDialog.ShowValidationError(FindForm(), taxValidation.ErrorMessage, "Checkout");
            return;
        }

        var paymentLabel = paymentMethod == PaymentMethod.Cash ? "Cash" : "Card";
        var totals = _posService.CalculateTotals(numDiscount.Value, numTax.Value);

        if (!ErrorDialog.Confirm(FindForm(), $"Complete {paymentLabel} payment for {totals.FinalAmount:C2}?", "Confirm Checkout"))
        {
            return;
        }

        SetBusyState(true);
        try
        {
            var result = await _posService.CheckoutAsync(numDiscount.Value, numTax.Value, paymentMethod);
            if (!result.IsSuccess)
            {
                ErrorDialog.ShowValidationError(FindForm(), result.Message, "Checkout");
                return;
            }

            numDiscount.Value = 0;
            numTax.Value = 0;
            RefreshCart();
            ErrorDialog.ShowInformation(FindForm(), result.Message, "Checkout");

            if (result.Data > 0)
            {
                await OfferReceiptPrintAsync(result.Data);
            }
        }
        catch (Exception ex)
        {
            AppLogging.LogError("Error during checkout.", ex);
            ErrorDialog.ShowError(FindForm(), ExceptionMapper.GetUserMessage(ex, "checkout"), "Checkout");
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private PosCartItemDto? GetSelectedCartItem()
    {
        return dgvCart.CurrentRow?.DataBoundItem as PosCartItemDto;
    }

    private void SyncSelectedCartItemQuantity()
    {
        var item = GetSelectedCartItem();
        if (item is null)
        {
            return;
        }

        numCartQuantity.Value = Math.Max(numCartQuantity.Minimum, Math.Min(numCartQuantity.Maximum, item.Quantity));
    }

    private void RefreshCart()
    {
        dgvCart.DataSource = null;
        dgvCart.DataSource = _posService.CartItems.ToList();
        lblCartCount.Text = $"{_posService.CartItems.Count} item(s)";
        btnRemoveItem.Enabled = _posService.CartItems.Count > 0;
        btnClearCart.Enabled = _posService.CartItems.Count > 0;
        btnUpdateQty.Enabled = _posService.CartItems.Count > 0;
        btnPayCash.Enabled = _posService.CartItems.Count > 0;
        btnPayCard.Enabled = _posService.CartItems.Count > 0;
        RefreshTotals();
    }

    private async Task OfferReceiptPrintAsync(int invoiceId)
    {
        if (!ErrorDialog.Confirm(FindForm(), "Would you like to print the receipt?", "Print Receipt"))
        {
            return;
        }

        try
        {
            var invoice = await _invoiceService.GetInvoiceDetailAsync(invoiceId);
            if (invoice is null)
            {
                ErrorDialog.ShowWarning(FindForm(), "Unable to load invoice for printing.", "Print Receipt");
                return;
            }

            ReceiptPrinter.ShowPreview(invoice, ReceiptFormat.Thermal, FindForm());
        }
        catch (Exception ex)
        {
            AppLogging.LogError("Error loading invoice for printing.", ex);
            ErrorDialog.ShowError(FindForm(), ExceptionMapper.GetUserMessage(ex, "printing receipt"), "Print Receipt");
        }
    }

    private void ShowLookupError(string message)
    {
        lblLookupStatus.ForeColor = UiTheme.Error;
        lblLookupStatus.Text = message;
    }

    private void RefreshTotals()
    {
        var totals = _posService.CalculateTotals(numDiscount.Value, numTax.Value);
        lblSubtotalValue.Text = totals.TotalAmount.ToString("C2");
        lblFinalTotalValue.Text = totals.FinalAmount.ToString("C2");
    }

    private void SetBusyState(bool isBusy)
    {
        UseWaitCursor = isBusy;
        txtBarcode.Enabled = !isBusy;
        btnLookup.Enabled = !isBusy;
        btnAddToCart.Enabled = !isBusy;
        btnPayCash.Enabled = !isBusy && _posService.CartItems.Count > 0;
        btnPayCard.Enabled = !isBusy && _posService.CartItems.Count > 0;
    }
}
