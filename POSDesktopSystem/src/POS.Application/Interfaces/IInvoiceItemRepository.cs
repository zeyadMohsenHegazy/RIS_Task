using POS.Domain.Entities;

namespace POS.Application.Interfaces;

public interface IInvoiceItemRepository : IRepository<InvoiceItem>
{
    Task<IReadOnlyList<InvoiceItem>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken = default);
}
