using Inventory.Application.DTOs;

namespace Inventory.Application.Interfaces;

public interface IAuditLogService
{
    Task<IEnumerable<AuditLogDto>> GetAllLogsAsync();
    Task<IEnumerable<AuditLogDto>> GetLogsByUserIdAsync(Guid userId);
    Task<IEnumerable<AuditLogDto>> GetLogsByActionAsync(string action);
}
