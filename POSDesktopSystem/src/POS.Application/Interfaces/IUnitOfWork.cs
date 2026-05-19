namespace POS.Application.Interfaces;

public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    IProductRepository Products { get; }
    IInvoiceRepository Invoices { get; }
    IInvoiceItemRepository InvoiceItems { get; }
    IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
