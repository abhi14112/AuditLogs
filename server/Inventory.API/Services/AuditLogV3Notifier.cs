using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Inventory.API.Services;

/// <summary>
/// SignalR implementation of audit log notifier for real-time updates
/// </summary>
public class AuditLogV3Notifier : IAuditLogV3Notifier
{
    private readonly IHubContext<AuditLogV3Hub> _hubContext;

    public AuditLogV3Notifier(IHubContext<AuditLogV3Hub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyAuditLogCreatedAsync(AuditLogV3Dto auditLog)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("ReceiveAuditLogV3", auditLog);
        }
        catch
        {
            // SignalR broadcast failed, but don't throw - just log if needed
            // This ensures audit log creation doesn't fail if SignalR has issues
        }
    }
}
