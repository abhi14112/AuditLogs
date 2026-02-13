using System.Text.Json;
using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogV2Service _auditLogV2Service;
    private readonly IAuditLogV3Service _auditLogV3Service;

    public ProductService(IUnitOfWork unitOfWork, IAuditLogV2Service auditLogV2Service, IAuditLogV3Service auditLogV3Service)
    {
        _unitOfWork = unitOfWork;
        _auditLogV2Service = auditLogV2Service;
        _auditLogV3Service = auditLogV3Service;
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

    public async Task<ProductDto> CreateProductAsync(CreateProductDto createProductDto, Guid userId, string username, string userEmail, string ipAddress)
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

        // Create audit log (V1)
        await CreateAuditLog(userId, "Create", "Product", product.Id.ToString(), null, product, ipAddress);

        // Create audit log (V2)
        await _auditLogV2Service.LogAsync(
            actionType: "Created",
            entityName: "Product",
            entityId: product.Id.ToString(),
            oldValues: null,
            newValues: new { Name = product.Name, Description = product.Description, Quantity = product.Quantity, Price = product.Price },
            message: $"Created Product {product.Name}");

        // Create audit log (V3)
        await _auditLogV3Service.LogActionAsync(
            eventType: "ProductCreated",
            entityType: "Product",
            entityId: product.Id.ToString(),
            entityName: product.Name,
            action: "Create",
            oldValues: null,
            newValues: new { Name = product.Name, Description = product.Description, Quantity = product.Quantity, Price = product.Price },
            userId: userId,
            username: username,
            userEmail: userEmail,
            ipAddress: ipAddress,
            userAgent: null,
            source: "Web",
            severity: "Medium",
            correlationId: null);

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

    public async Task<ProductDto> UpdateProductAsync(Guid id, UpdateProductDto updateProductDto, Guid userId, string username, string userEmail, string ipAddress)
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

        // Create audit log (V1)
        await CreateAuditLog(userId, "Update", "Product", product.Id.ToString(), oldProduct, product, ipAddress);

        // Create audit log (V2)
        await _auditLogV2Service.LogAsync(
            actionType: "Updated",
            entityName: "Product",
            entityId: product.Id.ToString(),
            oldValues: new { Name = oldProduct.Name, Description = oldProduct.Description, Quantity = oldProduct.Quantity, Price = oldProduct.Price },
            newValues: new { Name = product.Name, Description = product.Description, Quantity = product.Quantity, Price = product.Price },
            message: $"Updated Product {product.Name}");

        // Create audit log (V3)
        await _auditLogV3Service.LogActionAsync(
            eventType: "ProductUpdated",
            entityType: "Product",
            entityId: product.Id.ToString(),
            entityName: product.Name,
            action: "Update",
            oldValues: new { Name = oldProduct.Name, Description = oldProduct.Description, Quantity = oldProduct.Quantity, Price = oldProduct.Price },
            newValues: new { Name = product.Name, Description = product.Description, Quantity = product.Quantity, Price = product.Price },
            userId: userId,
            username: username,
            userEmail: userEmail,
            ipAddress: ipAddress,
            userAgent: null,
            source: "Web",
            severity: "Medium",
            correlationId: null);

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

    public async Task<bool> DeleteProductAsync(Guid id, Guid userId, string username, string userEmail, string ipAddress)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(id);
        if (product == null)
        {
            return false;
        }

        // Create audit log before deleting (V1)
        await CreateAuditLog(userId, "Delete", "Product", product.Id.ToString(), product, null, ipAddress);

        // Create audit log (V2)
        await _auditLogV2Service.LogAsync(
            actionType: "Deleted",
            entityName: "Product",
            entityId: product.Id.ToString(),
            oldValues: new { Name = product.Name, Description = product.Description, Quantity = product.Quantity, Price = product.Price },
            newValues: null,
            message: $"Deleted Product {product.Name}");

        // Create audit log (V3)
        await _auditLogV3Service.LogActionAsync(
            eventType: "ProductDeleted",
            entityType: "Product",
            entityId: product.Id.ToString(),
            entityName: product.Name,
            action: "Delete",
            oldValues: new { Name = product.Name, Description = product.Description, Quantity = product.Quantity, Price = product.Price },
            newValues: null,
            userId: userId,
            username: username,
            userEmail: userEmail,
            ipAddress: ipAddress,
            userAgent: null,
            source: "Web",
            severity: "High",
            correlationId: null);

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
