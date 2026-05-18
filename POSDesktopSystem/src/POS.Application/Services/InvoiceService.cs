using POS.Application.DTOs;
using POS.Application.Interfaces;
using POS.Application.Models;
using POS.Domain.Entities;
using POS.Domain.Enums;

namespace POS.Application.Services;

public sealed class InvoiceService : IInvoiceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAppLogger _logger;
    private readonly IDatabaseInfo _databaseInfo;

    public InvoiceService(
        IUnitOfWork unitOfWork,
        IAppLogger logger,
        IDatabaseInfo databaseInfo)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _databaseInfo = databaseInfo ?? throw new ArgumentNullException(nameof(databaseInfo));
    }

    public async Task<ServiceResult<int>> SaveInvoiceAsync(
        CheckoutRequest request,
        InvoiceCalculationDto calculation,
        int userId,
        CancellationToken cancellationToken = default)
    {
        var stageResult = await StageInvoiceAsync(request, calculation, userId, cancellationToken);
        if (!stageResult.IsSuccess || stageResult.Data is null)
        {
            return ServiceResult<int>.Failure(stageResult.Message);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var invoice = stageResult.Data;
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        LogCreatedInvoice(invoice, userId, user?.Username ?? "Unknown", request.Items.Count);

        return ServiceResult<int>.Success(invoice.Id, $"Invoice #{invoice.Id} saved successfully.");
    }

    public async Task<ServiceResult<Invoice>> StageInvoiceAsync(
        CheckoutRequest request,
        InvoiceCalculationDto calculation,
        int userId,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(calculation);

        if (userId <= 0)
        {
            return ServiceResult<Invoice>.Failure("A valid user is required to save an invoice.");
        }

        if (request.Items.Count == 0)
        {
            return ServiceResult<Invoice>.Failure("Invoice must contain at least one item.");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return ServiceResult<Invoice>.Failure("User not found.");
        }

        var invoice = InvoiceBuilder.Build(request, calculation, userId);
        invoice.SyncStatus = _databaseInfo.RequiresSync ? SyncStatus.Pending : SyncStatus.NotRequired;
        await _unitOfWork.Invoices.AddAsync(invoice, cancellationToken);

        return ServiceResult<Invoice>.Success(invoice);
    }

    public void LogCreatedInvoice(Invoice invoice, int userId, string username, int itemCount)
    {
        _logger.LogAction(
            "Invoice created",
            $"InvoiceId={invoice.Id}; UserId={userId}; Username={username}; " +
            $"Items={itemCount}; Subtotal={invoice.TotalAmount:C2}; " +
            $"Discount={invoice.Discount:C2}; Tax={invoice.Tax:C2}; " +
            $"Final={invoice.FinalAmount:C2}; Payment={invoice.PaymentMethod}");
    }

    public async Task<IReadOnlyList<InvoiceSummaryDto>> SearchInvoicesAsync(
        string? searchTerm,
        CancellationToken cancellationToken = default)
    {
        var rows = await _unitOfWork.Invoices.SearchSummaryRowsAsync(searchTerm, cancellationToken);
        return rows.Select(MapToSummary).ToList();
    }

    public async Task<InvoiceDetailDto?> GetInvoiceDetailAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdWithDetailsAsync(id, cancellationToken);
        return invoice is null ? null : MapToDetail(invoice);
    }

    private static InvoiceSummaryDto MapToSummary(InvoiceSearchRow row) =>
        new()
        {
            Id = row.Id,
            Date = row.Date,
            CreatedAt = row.CreatedAt,
            CashierName = row.CashierName,
            TotalAmount = row.TotalAmount,
            Discount = row.Discount,
            Tax = row.Tax,
            FinalAmount = row.FinalAmount,
            PaymentMethod = GetPaymentMethodLabel(row.PaymentMethod),
            ItemCount = row.ItemCount,
            SyncStatus = SyncStatusFormatter.GetLabel(row.SyncStatus)
        };

    private static InvoiceDetailDto MapToDetail(Invoice invoice) =>
        new()
        {
            Id = invoice.Id,
            Date = invoice.Date,
            CreatedAt = invoice.CreatedAt,
            CashierName = invoice.User?.Username ?? "Unknown",
            TotalAmount = invoice.TotalAmount,
            Discount = invoice.Discount,
            Tax = invoice.Tax,
            FinalAmount = invoice.FinalAmount,
            PaymentMethod = GetPaymentMethodLabel(invoice.PaymentMethod),
            SyncStatus = SyncStatusFormatter.GetLabel(invoice.SyncStatus),
            SyncedAt = invoice.SyncedAt,
            SyncError = invoice.SyncError,
            Items = invoice.Items
                .OrderBy(item => item.Id)
                .Select(item => new InvoiceItemDetailDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "Unknown",
                    Barcode = item.Product?.Barcode ?? string.Empty,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.SubTotal
                })
                .ToList()
        };

    private static string GetPaymentMethodLabel(PaymentMethod paymentMethod) =>
        paymentMethod switch
        {
            PaymentMethod.Cash => "Cash",
            PaymentMethod.Card => "Card",
            _ => paymentMethod.ToString()
        };
}
