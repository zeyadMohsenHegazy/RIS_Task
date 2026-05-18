using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventorySystem.API.Extensions;
using SmartInventorySystem.Application.DTOs.Common;
using SmartInventorySystem.Application.DTOs.Inventory;
using SmartInventorySystem.Application.Interfaces;

namespace SmartInventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class InventoryController : ControllerBase
{
    private readonly IInventoryService _inventoryService;

    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }

    /// <summary>Records stock in (increases product quantity). Admin only.</summary>
    [HttpPost("in")]
    [Authorize(Policy = AuthenticationExtensions.AdminOnlyPolicy)]
    [ProducesResponseType(typeof(InventoryTransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryTransactionDto>> StockIn(
        [FromBody] InventoryMovementDto dto,
        CancellationToken cancellationToken)
    {
        return await ExecuteMovementAsync(
            () => _inventoryService.StockInAsync(dto, cancellationToken));
    }

    /// <summary>Records stock out (decreases product quantity). Admin only.</summary>
    [HttpPost("out")]
    [Authorize(Policy = AuthenticationExtensions.AdminOnlyPolicy)]
    [ProducesResponseType(typeof(InventoryTransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryTransactionDto>> StockOut(
        [FromBody] InventoryMovementDto dto,
        CancellationToken cancellationToken)
    {
        return await ExecuteMovementAsync(
            () => _inventoryService.StockOutAsync(dto, cancellationToken));
    }

    /// <summary>Gets inventory transaction history with pagination. Search filters by product name.</summary>
    /// <param name="query">pageNumber=1, pageSize=10, search=laptop</param>
    [HttpGet("history")]
    [Authorize(Policy = AuthenticationExtensions.AdminOrEmployeePolicy)]
    [ProducesResponseType(typeof(PagedResponse<InventoryTransactionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<InventoryTransactionDto>>> GetHistory(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        var history = await _inventoryService.GetHistoryAsync(query, cancellationToken);
        return Ok(history);
    }

    private async Task<ActionResult<InventoryTransactionDto>> ExecuteMovementAsync(
        Func<Task<InventoryTransactionDto>> action)
    {
        try
        {
            var result = await action();
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
