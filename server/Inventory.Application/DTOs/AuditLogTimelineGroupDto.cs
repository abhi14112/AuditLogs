namespace Inventory.Application.DTOs;

public class AuditLogTimelineGroupDto
{
    public string Date { get; set; } = string.Empty;
    public List<AuditLogTimelineDto> Logs { get; set; } = new();
}
