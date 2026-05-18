using SmartInventorySystem.Application.Common;
using SmartInventorySystem.Application.DTOs.Common;

namespace SmartInventorySystem.Application.Mappings;

public static class PagedMapper
{
    public static PagedResponse<TDto> ToPagedResponse<TEntity, TDto>(
        PagedResult<TEntity> result,
        Func<TEntity, TDto> mapper)
    {
        var items = result.Items.Select(mapper).ToList();
        return PagedResponse<TDto>.Create(
            items,
            result.PageNumber,
            result.PageSize,
            result.TotalCount);
    }
}
