namespace Inventory.Domain.Entities;

/// <summary>
/// AuditLogV3 - Enterprise-level Jira-style audit logging entity
/// </summary>
public class AuditLogV3
{
    public Guid Id { get; set; }
    
    // Event Information
    public string EventType { get; set; } = string.Empty; // ProductCreated, ProductUpdated, UserLoggedIn, etc.
    public string EntityType { get; set; } = string.Empty; // Product, User, Inventory, etc.
    public string EntityId { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty; // Name of the entity being audited
    public string Action { get; set; } = string.Empty; // Create, Update, Delete, Login, Logout
    public string Description { get; set; } = string.Empty; // Human-readable description
    
    // Change Tracking
    public string? OldValues { get; set; } // JSON serialized old values
    public string? NewValues { get; set; } // JSON serialized new values
    public string? ChangesSummary { get; set; } // Summary of changes (e.g., "Price: 500 → 700, Quantity: 10 → 15")
    
    // User Information
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    
    // Request Context
    public string IpAddress { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public string Source { get; set; } = string.Empty; // Web, API, System, Mobile
    
    // Metadata
    public DateTime CreatedAt { get; set; }
    public string? CorrelationId { get; set; } // For tracking related operations
    public string Status { get; set; } = string.Empty; // Success, Failed, Pending
    public string Severity { get; set; } = string.Empty; // Low, Medium, High, Critical
}
