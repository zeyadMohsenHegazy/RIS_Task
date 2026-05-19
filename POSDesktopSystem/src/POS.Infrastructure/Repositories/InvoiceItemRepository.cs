using Microsoft.EntityFrameworkCore;
using POS.Application.Interfaces;
using POS.Domain.Entities;
using POS.Infrastructure.Data;

namespace POS.Infrastructure.Repositories;

public class InvoiceItemRepository : BaseRepository<InvoiceItem>, IInvoiceItemRepository
{
    public InvoiceItemRepository(POSDbContext context)
        : base(context)
    {
    }

    public async Task<IReadOnlyList<InvoiceItem>> GetByInvoiceIdAsync(
        int invoiceId,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .Where(ii => ii.InvoiceId == invoiceId)
            .ToListAsync(cancellationToken);
    }
}
