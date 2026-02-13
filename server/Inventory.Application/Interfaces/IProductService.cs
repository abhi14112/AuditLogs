using Inventory.Application.DTOs;

namespace Inventory.Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(Guid id);
    Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, Guid userId, string username, string userEmail, string ipAddress);
    Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto, Guid userId, string username, string userEmail, string ipAddress);
    Task<bool> DeleteProductAsync(Guid id, Guid userId, string username, string userEmail, string ipAddress);
}
