using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventorySystem.API.Extensions;
using SmartInventorySystem.Application.DTOs.Warehouses;
using SmartInventorySystem.Application.Interfaces;

namespace SmartInventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthenticationExtensions.AdminOrEmployeePolicy)]
[Produces("application/json")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    /// <summary>Gets all warehouses.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<WarehouseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<IReadOnlyList<WarehouseDto>>> GetAll(CancellationToken cancellationToken)
    {
        var warehouses = await _warehouseService.GetAllAsync(cancellationToken);
        return Ok(warehouses);
    }

    /// <summary>Creates a new warehouse. Admin only.</summary>
    [HttpPost]
    [Authorize(Policy = AuthenticationExtensions.AdminOnlyPolicy)]
    [ProducesResponseType(typeof(WarehouseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<WarehouseDto>> Create(
        [FromBody] CreateWarehouseDto dto,
        CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseService.CreateAsync(dto, cancellationToken);
        return Created($"/api/warehouses/{warehouse.Id}", warehouse);
    }
}
