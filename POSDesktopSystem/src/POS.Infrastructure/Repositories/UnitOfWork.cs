using POS.Application.Interfaces;
using POS.Infrastructure.Data;

namespace POS.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly POSDbContext _context;

    public UnitOfWork(
        POSDbContext context,
        IProductRepository products,
        IInvoiceRepository invoices,
        IInvoiceItemRepository invoiceItems,
        IUserRepository users)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Products = products ?? throw new ArgumentNullException(nameof(products));
        Invoices = invoices ?? throw new ArgumentNullException(nameof(invoices));
        InvoiceItems = invoiceItems ?? throw new ArgumentNullException(nameof(invoiceItems));
        Users = users ?? throw new ArgumentNullException(nameof(users));
    }

    public IProductRepository Products { get; }
    public IInvoiceRepository Invoices { get; }
    public IInvoiceItemRepository InvoiceItems { get; }
    public IUserRepository Users { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _context.DisposeAsync();
    }
}
