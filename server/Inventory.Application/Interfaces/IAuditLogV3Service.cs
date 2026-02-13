using Inventory.Application.DTOs;

namespace Inventory.Application.Interfaces;

/// <summary>
/// Service interface for AuditLogV3 operations
/// </summary>
public interface IAuditLogV3Service
{
    Task<AuditLogV3Dto> CreateAsync(CreateAuditLogV3Dto createDto);
    Task<AuditLogV3Dto?> GetByIdAsync(Guid id);
    Task<IEnumerable<AuditLogV3Dto>> GetAllAsync();
    Task<PagedAuditLogV3Dto> GetPagedAsync(
        int page = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? entityType = null,
        string? action = null,
        string? severity = null,
        Guid? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? sortBy = null,
        bool sortDescending = true
    );
    Task<IEnumerable<AuditLogV3Dto>> GetByEntityAsync(string entityId, string? entityType = null);
    Task<IEnumerable<AuditLogV3Dto>> GetByUserAsync(Guid userId, int top = 100);
    Task<IEnumerable<AuditLogV3Dto>> GetByCorrelationIdAsync(string correlationId);
    Task<IEnumerable<AuditLogV3Dto>> GetBySeverityAsync(string severity);
    
    // Helper method for automatic logging
    Task LogActionAsync(
        string eventType,
        string entityType,
        string entityId,
        string entityName,
        string action,
        object? oldValues,
        object? newValues,
        Guid userId,
        string username,
        string userEmail,
        string ipAddress,
        string? userAgent = null,
        string source = "Web",
        string severity = "Medium",
        string? correlationId = null
    );
}
