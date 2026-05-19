using POS.Application.Models;
using POS.Domain.Entities;
namespace POS.Application.Interfaces;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<Invoice?> GetByIdWithItemsAsync(int id, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Invoice>> SearchAsync(string? searchTerm, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<InvoiceSearchRow>> SearchSummaryRowsAsync(string? searchTerm, CancellationToken cancellationToken = default);
}
