namespace Inventory.Domain.Entities;

public class AuditLogV2
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string ActionType { get; set; } = string.Empty; // Created, Updated, Deleted, Login, Logout
    public string EntityName { get; set; } = string.Empty; // Product, Inventory, User
    public string EntityId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty; // "Abhishek updated Product Price"
    public DateTime Timestamp { get; set; }
    public string? OldValues { get; set; } // JSON serialized
    public string? NewValues { get; set; } // JSON serialized
    public string? ChangedFields { get; set; } // JSON array of changed field names
    public string IpAddress { get; set; } = string.Empty;
}
