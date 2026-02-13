namespace Inventory.Application.DTOs;

public class AuditLogTimelineDto
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty;
}
