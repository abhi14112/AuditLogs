namespace Inventory.Application.DTOs;

public class AuditLogDetailDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public object? OldValues { get; set; }
    public object? NewValues { get; set; }
    public List<string> ChangedFields { get; set; } = new();
    public string IpAddress { get; set; } = string.Empty;
}
