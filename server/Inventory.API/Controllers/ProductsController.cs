using System.Security.Claims;
using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProductById(Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound(new { message = "Product not found" });
        }
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> CreateProduct([FromBody] CreateProductDto createProductDto)
    {
        var userId = GetCurrentUserId();
        var ipAddress = GetClientIpAddress();

        var product = await _productService.CreateProductAsync(createProductDto, userId, ipAddress);
        return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, [FromBody] UpdateProductDto updateProductDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var ipAddress = GetClientIpAddress();

            var product = await _productService.UpdateProductAsync(id, updateProductDto, userId, ipAddress);
            return Ok(product);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        var userId = GetCurrentUserId();
        var ipAddress = GetClientIpAddress();

        var result = await _productService.DeleteProductAsync(id, userId, ipAddress);
        if (!result)
        {
            return NotFound(new { message = "Product not found" });
        }
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                          ?? User.FindFirst("sub")?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID not found in token"));
    }

    private string GetClientIpAddress()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}
