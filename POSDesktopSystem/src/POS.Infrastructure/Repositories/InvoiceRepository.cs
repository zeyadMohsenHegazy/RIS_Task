using Microsoft.EntityFrameworkCore;
using POS.Application.Interfaces;
using POS.Application.Models;
using POS.Domain.Entities;
using POS.Infrastructure.Data;

namespace POS.Infrastructure.Repositories;

public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(POSDbContext context)
        : base(context)
    {
    }

    public async Task<Invoice?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.Items)
            .ThenInclude(ii => ii.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<Invoice?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(i => i.User)
            .Include(i => i.Items)
            .ThenInclude(ii => ii.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Invoice>> SearchAsync(
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(i => i.User)
            .Include(i => i.Items)
            .AsQueryable();

        query = ApplySearchFilter(query, searchTerm);

        return await query
            .OrderByDescending(i => i.Date)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<InvoiceSearchRow>> SearchSummaryRowsAsync(
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .AsNoTracking()
            .Include(i => i.User)
            .AsQueryable();

        query = ApplySearchFilter(query, searchTerm);

        return await query
            .OrderByDescending(i => i.Date)
            .Select(i => new InvoiceSearchRow
            {
                Id = i.Id,
                Date = i.Date,
                CreatedAt = i.CreatedAt,
                CashierName = i.User.Username,
                TotalAmount = i.TotalAmount,
                Discount = i.Discount,
                Tax = i.Tax,
                FinalAmount = i.FinalAmount,
                PaymentMethod = i.PaymentMethod,
                ItemCount = i.Items.Count,
                SyncStatus = i.SyncStatus
            })
            .ToListAsync(cancellationToken);
    }

    private static IQueryable<Invoice> ApplySearchFilter(IQueryable<Invoice> query, string? searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return query;
        }

        var term = searchTerm.Trim();
        if (int.TryParse(term, out var invoiceId))
        {
            return query.Where(i => i.Id == invoiceId);
        }

        return query.Where(i => i.User.Username.Contains(term));
    }
}
