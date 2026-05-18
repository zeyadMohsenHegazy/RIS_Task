using POS.Application.DTOs;
using POS.Domain.Entities;

namespace POS.Application.Interfaces;

public interface IInvoiceService
{
    Task<ServiceResult<int>> SaveInvoiceAsync(
        CheckoutRequest request,
        InvoiceCalculationDto calculation,
        int userId,
        CancellationToken cancellationToken = default);

    Task<ServiceResult<Invoice>> StageInvoiceAsync(
        CheckoutRequest request,
        InvoiceCalculationDto calculation,
        int userId,
        CancellationToken cancellationToken = default);

    void LogCreatedInvoice(Invoice invoice, int userId, string username, int itemCount);

    Task<IReadOnlyList<InvoiceSummaryDto>> SearchInvoicesAsync(
        string? searchTerm,
        CancellationToken cancellationToken = default);

    Task<InvoiceDetailDto?> GetInvoiceDetailAsync(int id, CancellationToken cancellationToken = default);
}
