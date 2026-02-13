using Inventory.Application.DTOs;

namespace Inventory.Application.Interfaces;

public interface IAuditLogV2Service
{
    Task LogAsync(
        string actionType,
        string entityName,
        string entityId,
        object? oldValues,
        object? newValues,
        string message);

    Task<IEnumerable<AuditLogTimelineGroupDto>> GetTimelineAsync(DateTime? fromDate = null, DateTime? toDate = null);
    Task<AuditLogDetailDto?> GetDetailAsync(Guid id);
    Task<IEnumerable<AuditLogV2Dto>> FilterAsync(DateTime? fromDate, DateTime? toDate, string? userId, string? entityName);
}
