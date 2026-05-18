namespace SmartInventorySystem.Application.DTOs.Common;

/// <summary>Query parameters for paginated list endpoints.</summary>
public class PaginationQuery
{
    /// <summary>Page number (1-based). Example: 1</summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>Number of items per page (max 100). Example: 10</summary>
    public int PageSize { get; set; } = 10;

    /// <summary>Optional search keyword (product name). Example: laptop</summary>
    public string? Search { get; set; }

    public (int PageNumber, int PageSize) Normalize()
    {
        var pageNumber = PageNumber < 1 ? 1 : PageNumber;
        var pageSize = PageSize < 1 ? 10 : Math.Min(PageSize, 100);
        return (pageNumber, pageSize);
    }

    public string? NormalizedSearch() =>
        string.IsNullOrWhiteSpace(Search) ? null : Search.Trim();
}
