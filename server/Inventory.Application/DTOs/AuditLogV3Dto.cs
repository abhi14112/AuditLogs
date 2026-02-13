namespace Inventory.Application.DTOs;

/// <summary>
/// DTO for AuditLogV3 - Jira-style audit log data transfer object
/// </summary>
public class AuditLogV3Dto
{
    public Guid Id { get; set; }
    
    // Event Information
    public string EventType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Change Tracking
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? ChangesSummary { get; set; }
    
    // User Information
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    
    // Request Context
    public string IpAddress { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public string Source { get; set; } = string.Empty;
    
    // Metadata
    public DateTime CreatedAt { get; set; }
    public string? CorrelationId { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating a new AuditLogV3 entry
/// </summary>
public class CreateAuditLogV3Dto
{
    public string EventType { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? ChangesSummary { get; set; }
    
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    
    public string IpAddress { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public string Source { get; set; } = "Web";
    
    public string? CorrelationId { get; set; }
    public string Status { get; set; } = "Success";
    public string Severity { get; set; } = "Medium";
}

/// <summary>
/// DTO for paginated audit log results
/// </summary>
public class PagedAuditLogV3Dto
{
    public IEnumerable<AuditLogV3Dto> Items { get; set; } = new List<AuditLogV3Dto>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
