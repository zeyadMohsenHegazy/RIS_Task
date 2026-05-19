using Microsoft.EntityFrameworkCore;
using POS.Application.Interfaces;
using POS.Domain.Entities;
using POS.Domain.Enums;
using POS.Infrastructure.Data;

namespace POS.Infrastructure.Sync;

public sealed class EfInvoiceSyncService : IInvoiceSyncService
{
    private readonly IDatabaseConnectionFactory _connectionFactory;
    private readonly POSDbContext _localContext;

    public EfInvoiceSyncService(
        IDatabaseConnectionFactory connectionFactory,
        POSDbContext localContext)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        _localContext = localContext ?? throw new ArgumentNullException(nameof(localContext));
    }

    public async Task<SyncResult> SyncPendingInvoicesAsync(CancellationToken cancellationToken = default)
    {
        if (!_connectionFactory.RequiresSync)
        {
            return new SyncResult { Message = "Sync is not required for the current database provider." };
        }

        var pendingInvoices = await _localContext.Invoices
            .Include(i => i.User)
            .Include(i => i.Items)
            .ThenInclude(item => item.Product)
            .Where(i => i.SyncStatus == SyncStatus.Pending || i.SyncStatus == SyncStatus.Failed)
            .OrderBy(i => i.Id)
            .ToListAsync(cancellationToken);

        if (pendingInvoices.Count == 0)
        {
            return new SyncResult { Message = "No pending invoices to sync." };
        }

        var syncedCount = 0;
        var failedCount = 0;

        await using var remoteContext = _connectionFactory.CreateRemoteDbContext();

        foreach (var localInvoice in pendingInvoices)
        {
            try
            {
                await SyncSingleInvoiceAsync(remoteContext, localInvoice, cancellationToken);
                localInvoice.SyncStatus = SyncStatus.Synced;
                localInvoice.SyncedAt = DateTime.UtcNow;
                localInvoice.SyncError = null;
                syncedCount++;
            }
            catch (Exception ex)
            {
                localInvoice.SyncStatus = SyncStatus.Failed;
                localInvoice.SyncError = ex.Message;
                failedCount++;
            }
        }

        await _localContext.SaveChangesAsync(cancellationToken);

        return new SyncResult
        {
            SyncedCount = syncedCount,
            FailedCount = failedCount,
            Message = $"Synced {syncedCount} invoice(s), {failedCount} failed."
        };
    }

    private static async Task SyncSingleInvoiceAsync(
        POSDbContext remoteContext,
        Invoice localInvoice,
        CancellationToken cancellationToken)
    {
        var remoteUser = await remoteContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == localInvoice.User.Username, cancellationToken);

        if (remoteUser is null)
        {
            throw new InvalidOperationException(
                $"Remote user '{localInvoice.User.Username}' was not found. Seed users on the remote database first.");
        }

        var remoteInvoice = new Invoice
        {
            UserId = remoteUser.Id,
            Date = localInvoice.Date,
            CreatedAt = localInvoice.CreatedAt,
            TotalAmount = localInvoice.TotalAmount,
            Discount = localInvoice.Discount,
            Tax = localInvoice.Tax,
            FinalAmount = localInvoice.FinalAmount,
            PaymentMethod = localInvoice.PaymentMethod,
            SyncStatus = SyncStatus.NotRequired,
            Items = new List<InvoiceItem>()
        };

        foreach (var localItem in localInvoice.Items)
        {
            var barcode = localItem.Product?.Barcode;
            if (string.IsNullOrWhiteSpace(barcode))
            {
                throw new InvalidOperationException("Local invoice item is missing product barcode.");
            }

            var trackedProduct = await remoteContext.Products
                .FirstOrDefaultAsync(p => p.Barcode == barcode, cancellationToken);

            if (trackedProduct is null)
            {
                throw new InvalidOperationException(
                    $"Remote product with barcode '{barcode ?? "unknown"}' was not found.");
            }

            remoteInvoice.Items.Add(new InvoiceItem
            {
                ProductId = trackedProduct.Id,
                Quantity = localItem.Quantity,
                UnitPrice = localItem.UnitPrice,
                SubTotal = localItem.SubTotal
            });

            trackedProduct.StockQuantity -= localItem.Quantity;
        }

        await remoteContext.Invoices.AddAsync(remoteInvoice, cancellationToken);
        await remoteContext.SaveChangesAsync(cancellationToken);
    }
}
