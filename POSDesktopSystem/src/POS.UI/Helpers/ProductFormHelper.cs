using POS.Application.DTOs;
using POS.Application.Interfaces;
using POS.Application.Validators;
using POS.UI.Forms;

namespace POS.UI.Helpers;

public static class ProductFormHelper
{
    public static async Task<bool> CreateProductAsync(
        IWin32Window owner,
        IProductService productService,
        ProductValidator validator)
    {
        while (true)
        {
            using var form = new ProductForm(validator);
            if (form.ShowDialog(owner) != DialogResult.OK)
            {
                return false;
            }

            var result = await productService.CreateAsync(form.ProductInput);
            if (result.IsSuccess)
            {
                ErrorDialog.ShowInformation(owner, result.Message, "Products");
                return true;
            }

            using var retryForm = new ProductForm(validator, form.ProductInput, result.Message);
            if (retryForm.ShowDialog(owner) != DialogResult.OK)
            {
                return false;
            }

            var retryResult = await productService.CreateAsync(retryForm.ProductInput);
            if (retryResult.IsSuccess)
            {
                ErrorDialog.ShowInformation(owner, retryResult.Message, "Products");
                return true;
            }

            ErrorDialog.ShowValidationError(owner, retryResult.Message, "Products");
        }
    }

    public static async Task<bool> EditProductAsync(
        IWin32Window owner,
        IProductService productService,
        ProductValidator validator,
        ProductDto selected)
    {
        while (true)
        {
            using var form = new ProductForm(validator, selected);
            if (form.ShowDialog(owner) != DialogResult.OK)
            {
                return false;
            }

            var result = await productService.UpdateAsync(selected.Id, form.ProductInput);
            if (result.IsSuccess)
            {
                ErrorDialog.ShowInformation(owner, result.Message, "Products");
                return true;
            }

            using var retryForm = new ProductForm(validator, selected, form.ProductInput, result.Message);
            if (retryForm.ShowDialog(owner) != DialogResult.OK)
            {
                return false;
            }

            var retryResult = await productService.UpdateAsync(selected.Id, retryForm.ProductInput);
            if (retryResult.IsSuccess)
            {
                ErrorDialog.ShowInformation(owner, retryResult.Message, "Products");
                return true;
            }

            ErrorDialog.ShowValidationError(owner, retryResult.Message, "Products");
        }
    }
}
