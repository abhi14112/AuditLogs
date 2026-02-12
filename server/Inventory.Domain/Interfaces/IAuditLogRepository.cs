using Inventory.Domain.Entities;

namespace Inventory.Domain.Interfaces;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetLogsByUserIdAsync(Guid userId);
    Task<IEnumerable<AuditLog>> GetLogsByEntityAsync(string entityName, string entityId);
    Task<IEnumerable<AuditLog>> GetLogsByActionAsync(string action);
}
