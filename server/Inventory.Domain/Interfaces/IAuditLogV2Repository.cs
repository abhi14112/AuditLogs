using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IAuditLogV2Repository
{
    Task<AuditLogV2> AddAsync(AuditLogV2 auditLog);
    Task<AuditLogV2?> GetByIdAsync(Guid id);
    Task<IEnumerable<AuditLogV2>> GetAllAsync();
    Task<IEnumerable<AuditLogV2>> GetTimelineAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<IEnumerable<AuditLogV2>> FilterAsync(DateTime? fromDate, DateTime? toDate, string? userId, string? entityName);
}
