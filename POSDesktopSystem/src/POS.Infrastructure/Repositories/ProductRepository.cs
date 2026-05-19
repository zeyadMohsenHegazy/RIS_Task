using Microsoft.EntityFrameworkCore;
using POS.Application.Helpers;
using POS.Application.Interfaces;
using POS.Domain.Entities;
using POS.Infrastructure.Data;

namespace POS.Infrastructure.Repositories;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(POSDbContext context)
        : base(context)
    {
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
    {
        var normalized = BarcodeNormalizer.Normalize(barcode);
        if (string.IsNullOrEmpty(normalized))
        {
            return null;
        }

        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(
                p => p.Barcode.ToUpper() == normalized || p.Barcode == barcode.Trim(),
                cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> SearchByBarcodeAsync(
        string barcode,
        CancellationToken cancellationToken = default)
    {
        var normalized = BarcodeNormalizer.Normalize(barcode);
        if (string.IsNullOrEmpty(normalized))
        {
            return Array.Empty<Product>();
        }

        return await DbSet
            .AsNoTracking()
            .Where(p => p.Barcode.ToUpper().Contains(normalized))
            .OrderBy(p => p.Barcode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> SearchAsync(
        string searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            query = query.Where(p => p.Name.Contains(term) || p.Barcode.Contains(term));
        }

        return await query
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Product>> GetByIdsAsync(
        IEnumerable<int> productIds,
        CancellationToken cancellationToken = default)
    {
        var ids = productIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return Array.Empty<Product>();
        }

        return await DbSet
            .Where(p => ids.Contains(p.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> BarcodeExistsAsync(
        string barcode,
        int? excludeProductId = null,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet.AsNoTracking().Where(p => p.Barcode == barcode);

        if (excludeProductId.HasValue)
        {
            query = query.Where(p => p.Id != excludeProductId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> HasInvoiceItemsAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await Context.InvoiceItems
            .AsNoTracking()
            .AnyAsync(ii => ii.ProductId == productId, cancellationToken);
    }
}
