using POS.Application.DTOs;
using POS.Application.Interfaces;
using POS.Application.Validators;
using POS.UI.Helpers;

namespace POS.UI.Views;

public partial class ProductsView : UserControl
{
    private readonly IProductService _productService;
    private readonly ProductValidator _productValidator;
    private readonly SearchDebouncer _searchDebouncer;

    public ProductsView(IProductService productService, ProductValidator productValidator)
    {
        _productService = productService ?? throw new ArgumentNullException(nameof(productService));
        _productValidator = productValidator ?? throw new ArgumentNullException(nameof(productValidator));
        _searchDebouncer = new SearchDebouncer(RefreshProductsAsync);

        InitializeComponent();
        ConfigureGrid();
        WireEvents();
    }

    private void ConfigureGrid()
    {
        DataGridHelper.ConfigureReadOnlyGrid(dgvProducts);
        dgvProducts.Columns.Clear();
        DataGridHelper.AddTextColumn(dgvProducts, nameof(ProductDto.Id), "ID", 60);
        DataGridHelper.AddTextColumn(dgvProducts, nameof(ProductDto.Name), "Name", 180);
        DataGridHelper.AddTextColumn(dgvProducts, nameof(ProductDto.Barcode), "Barcode", 120);
        DataGridHelper.AddTextColumn(dgvProducts, nameof(ProductDto.Price), "Price", 80, "C2");
        DataGridHelper.AddTextColumn(dgvProducts, nameof(ProductDto.StockQuantity), "Stock", 70);
    }

    private void WireEvents()
    {
        Load += async (_, _) => await RefreshProductsAsync();
        txtSearch.TextChanged += (_, _) => _searchDebouncer.Schedule();
        btnAdd.Click += async (_, _) => await AddProductAsync();
        btnEdit.Click += async (_, _) => await EditProductAsync();
        btnDelete.Click += async (_, _) => await DeleteProductAsync();
        btnRefresh.Click += async (_, _) => await RefreshProductsAsync();
        dgvProducts.SelectionChanged += (_, _) => UpdateActionButtons();
        dgvProducts.CellDoubleClick += async (_, _) => await EditProductAsync();
    }

    private ProductDto? GetSelectedProduct() =>
        dgvProducts.CurrentRow?.DataBoundItem as ProductDto;

    private void UpdateActionButtons()
    {
        var hasSelection = GetSelectedProduct() is not null;
        btnEdit.Enabled = hasSelection;
        btnDelete.Enabled = hasSelection;
    }

    private async Task RefreshProductsAsync()
    {
        SetBusyState(true);
        try
        {
            var products = await _productService.SearchAsync(txtSearch.Text);
            dgvProducts.DataSource = products.ToList();
            lblStatus.Text = $"{products.Count} product(s) found";
            UpdateActionButtons();
        }
        catch (Exception ex)
        {
            lblStatus.Text = "Unable to load products.";
            AppLogging.LogError("Error loading products.", ex);
            ErrorDialog.ShowError(FindForm(), Exceptions.ExceptionMapper.GetUserMessage(ex, "loading products"));
        }
        finally
        {
            SetBusyState(false);
        }
    }

    private async Task AddProductAsync()
    {
        if (await ProductFormHelper.CreateProductAsync(FindForm()!, _productService, _productValidator))
        {
            await RefreshProductsAsync();
        }
    }

    private async Task EditProductAsync()
    {
        var selected = GetSelectedProduct();
        if (selected is null)
        {
            return;
        }

        if (await ProductFormHelper.EditProductAsync(FindForm()!, _productService, _productValidator, selected))
        {
            await RefreshProductsAsync();
        }
    }

    private async Task DeleteProductAsync()
    {
        var selected = GetSelectedProduct();
        if (selected is null)
        {
            return;
        }

        if (!ErrorDialog.Confirm(FindForm(), $"Delete product '{selected.Name}'?", "Confirm Delete"))
        {
            return;
        }

        var result = await _productService.DeleteAsync(selected.Id);
        if (!result.IsSuccess)
        {
            ErrorDialog.ShowValidationError(FindForm(), result.Message, "Products");
            return;
        }

        await RefreshProductsAsync();
        ErrorDialog.ShowInformation(FindForm(), result.Message, "Products");
    }

    private void SetBusyState(bool isBusy)
    {
        UseWaitCursor = isBusy;
        btnAdd.Enabled = !isBusy;
        btnEdit.Enabled = !isBusy && GetSelectedProduct() is not null;
        btnDelete.Enabled = !isBusy && GetSelectedProduct() is not null;
        btnRefresh.Enabled = !isBusy;
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
