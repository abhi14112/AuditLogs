using Inventory.Application.DTOs;

namespace Inventory.Application.Interfaces;

/// <summary>
/// Interface for notifying about audit log events (e.g., via SignalR)
/// </summary>
public interface IAuditLogV3Notifier
{
    Task NotifyAuditLogCreatedAsync(AuditLogV3Dto auditLog);
}
