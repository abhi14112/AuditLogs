using System.Text.Json;
using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _unitOfWork.Products.GetAllAsync();
        return products.Select(p => new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Quantity = p.Quantity,
            Price = p.Price,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        });
    }

    public async Task<ProductDto?> GetProductByIdAsync(Guid id)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            Price = product.Price,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, Guid userId, string ipAddress)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Quantity = createProductDto.Quantity,
            Price = createProductDto.Price,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Products.AddAsync(product);

        // Create audit log
        await CreateAuditLog(userId, "Create", "Product", product.Id.ToString(), null, product, ipAddress);

        await _unitOfWork.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            Price = product.Price,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto, Guid userId, string ipAddress)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found");
        }

        // Store old values for audit
        var oldProduct = new Product
        {
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            Price = product.Price
        };

        // Update product
        product.Name = updateProductDto.Name;
        product.Description = updateProductDto.Description;
        product.Quantity = updateProductDto.Quantity;
        product.Price = updateProductDto.Price;
        product.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Products.UpdateAsync(product);

        // Create audit log
        await CreateAuditLog(userId, "Update", "Product", product.Id.ToString(), oldProduct, product, ipAddress);

        await _unitOfWork.SaveChangesAsync();

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            Price = product.Price,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public async Task<bool> DeleteProductAsync(Guid id, Guid userId, string ipAddress)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
        {
            return false;
        }

        // Create audit log before deleting
        await CreateAuditLog(userId, "Delete", "Product", product.Id.ToString(), product, null, ipAddress);

        await _unitOfWork.Products.DeleteAsync(product);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    private async Task CreateAuditLog(Guid userId, string action, string entityName, string entityId, 
        object? oldValues, object? newValues, string ipAddress)
    {
        var auditLog = new AuditLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            Timestamp = DateTime.UtcNow,
            IpAddress = ipAddress
        };

        await _unitOfWork.AuditLogs.AddAsync(auditLog);
    }
}
