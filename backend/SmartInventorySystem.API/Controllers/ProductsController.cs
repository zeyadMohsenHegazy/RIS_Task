using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventorySystem.API.Extensions;
using SmartInventorySystem.Application.DTOs.Common;
using SmartInventorySystem.Application.DTOs.Products;
using SmartInventorySystem.Application.Interfaces;

namespace SmartInventorySystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = AuthenticationExtensions.AdminOrEmployeePolicy)]
[Produces("application/json")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    /// <summary>Gets products with pagination and optional search by name.</summary>
    /// <param name="query">pageNumber=1, pageSize=10, search=laptop</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<ProductDto>>> GetAll(
        [FromQuery] PaginationQuery query,
        CancellationToken cancellationToken)
    {
        var products = await _productService.GetPagedAsync(query, cancellationToken);
        return Ok(products);
    }

    /// <summary>Gets a product by id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product is null)
        {
            return NotFound(new { message = $"Product with id {id} was not found." });
        }

        return Ok(product);
    }

    /// <summary>Creates a new product. Admin only.</summary>
    [HttpPost]
    [Authorize(Policy = AuthenticationExtensions.AdminOnlyPolicy)]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateProductDto dto,
        CancellationToken cancellationToken)
    {
        var product = await _productService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>Updates an existing product. Admin only.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Policy = AuthenticationExtensions.AdminOnlyPolicy)]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductDto>> Update(
        int id,
        [FromBody] UpdateProductDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.UpdateAsync(id, dto, cancellationToken);
            if (product is null)
            {
                return NotFound(new { message = $"Product with id {id} was not found." });
            }

            return Ok(product);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Deletes a product. Admin only.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Policy = AuthenticationExtensions.AdminOnlyPolicy)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _productService.DeleteAsync(id, cancellationToken);
            if (!deleted)
            {
                return NotFound(new { message = $"Product with id {id} was not found." });
            }

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
