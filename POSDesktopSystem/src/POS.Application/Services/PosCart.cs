using POS.Application.DTOs;

namespace POS.Application.Services;

public sealed class PosCart
{
    private readonly List<PosCartItemDto> _items = [];

    public IReadOnlyList<PosCartItemDto> Items => _items;

    public ServiceResult AddItem(ProductDto product, int quantity)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (quantity <= 0)
        {
            return ServiceResult.Failure("Quantity must be greater than zero.");
        }

        var existingItem = _items.FirstOrDefault(item => item.ProductId == product.Id);
        var newQuantity = (existingItem?.Quantity ?? 0) + quantity;

        if (newQuantity > product.StockQuantity)
        {
            return ServiceResult.Failure(
                $"Insufficient stock for '{product.Name}'. Available: {product.StockQuantity}.");
        }

        if (existingItem is not null)
        {
            existingItem.Quantity = newQuantity;
            existingItem.AvailableStock = product.StockQuantity;
            existingItem.UnitPrice = product.Price;
        }
        else
        {
            _items.Add(new PosCartItemDto
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Barcode = product.Barcode,
                Quantity = quantity,
                UnitPrice = product.Price,
                AvailableStock = product.StockQuantity
            });
        }

        return ServiceResult.Success("Product added to cart.");
    }

    public ServiceResult UpdateQuantity(int productId, int quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
        {
            return ServiceResult.Failure("Product is not in the cart.");
        }

        if (quantity <= 0)
        {
            return ServiceResult.Failure("Quantity must be greater than zero.");
        }

        if (quantity > item.AvailableStock)
        {
            return ServiceResult.Failure(
                $"Insufficient stock for '{item.ProductName}'. Available: {item.AvailableStock}.");
        }

        item.Quantity = quantity;
        return ServiceResult.Success("Cart item updated.");
    }

    public ServiceResult RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
        {
            return ServiceResult.Failure("Product is not in the cart.");
        }

        _items.Remove(item);
        return ServiceResult.Success("Product removed from cart.");
    }

    public void Clear()
    {
        _items.Clear();
    }
}
