using Microsoft.AspNetCore.SignalR;

namespace Inventory.API.Hubs;

/// <summary>
/// SignalR Hub for real-time AuditLogV3 updates
/// </summary>
public class AuditLogV3Hub : Hub
{
    /// <summary>
    /// Called when a client connects to the hub
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.SendAsync("Connected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    /// <summary>
    /// Called when a client disconnects from the hub
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Broadcast audit log to all connected clients
    /// </summary>
    public async Task BroadcastAuditLog(object auditLog)
    {
        await Clients.All.SendAsync("ReceiveAuditLogV3", auditLog);
    }

    /// <summary>
    /// Join a specific entity's audit room
    /// </summary>
    public async Task JoinEntityRoom(string entityId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Entity_{entityId}");
        await Clients.Caller.SendAsync("JoinedRoom", $"Entity_{entityId}");
    }

    /// <summary>
    /// Leave a specific entity's audit room
    /// </summary>
    public async Task LeaveEntityRoom(string entityId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Entity_{entityId}");
        await Clients.Caller.SendAsync("LeftRoom", $"Entity_{entityId}");
    }

    /// <summary>
    /// Join a specific user's audit room
    /// </summary>
    public async Task JoinUserRoom(string userId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
        await Clients.Caller.SendAsync("JoinedRoom", $"User_{userId}");
    }

    /// <summary>
    /// Leave a specific user's audit room
    /// </summary>
    public async Task LeaveUserRoom(string userId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
        await Clients.Caller.SendAsync("LeftRoom", $"User_{userId}");
    }
}
