using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

/// <summary>
/// Repository interface for AuditLogV3 operations
/// </summary>
public interface IAuditLogV3Repository
{
    Task<AuditLogV3> CreateAsync(AuditLogV3 auditLog);
    Task<AuditLogV3?> GetByIdAsync(Guid id);
    Task<IEnumerable<AuditLogV3>> GetAllAsync();
    Task<(IEnumerable<AuditLogV3> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
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
    Task<IEnumerable<AuditLogV3>> GetByEntityAsync(string entityId, string? entityType = null);
    Task<IEnumerable<AuditLogV3>> GetByUserAsync(Guid userId, int top = 100);
    Task<IEnumerable<AuditLogV3>> GetByCorrelationIdAsync(string correlationId);
    Task<IEnumerable<AuditLogV3>> GetBySeverityAsync(string severity);
}
