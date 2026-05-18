using SmartInventorySystem.Domain.Enums;

namespace SmartInventorySystem.Application.DTOs.Inventory;

/// <summary>Query parameters for inventory transaction history.</summary>
public class InventoryHistoryQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public TransactionType? TransactionType { get; set; }

    public (int PageNumber, int PageSize) Normalize()
    {
        var pageNumber = PageNumber < 1 ? 1 : PageNumber;
        var pageSize = PageSize < 1 ? 10 : Math.Min(PageSize, 100);
        return (pageNumber, pageSize);
    }

    public string? NormalizedSearch() =>
        string.IsNullOrWhiteSpace(Search) ? null : Search.Trim();
}
