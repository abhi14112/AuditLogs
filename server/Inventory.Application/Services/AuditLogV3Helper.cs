using Inventory.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Inventory.Application.Services;

/// <summary>
/// Helper service for automatic audit logging in Audit Log V3
/// </summary>
public class AuditLogV3Helper
{
    private readonly IAuditLogV3Service _auditLogService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogV3Helper(
        IAuditLogV3Service auditLogService,
        IHttpContextAccessor httpContextAccessor)
    {
        _auditLogService = auditLogService;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Log a create action
    /// </summary>
    public async Task LogCreateAsync<T>(string entityId, string entityName, T newEntity, string? correlationId = null)
    {
        await LogActionInternalAsync(
            eventType: $"{typeof(T).Name}Created",
            entityType: typeof(T).Name,
            entityId: entityId,
            entityName: entityName,
            action: "Create",
            oldValues: null,
            newValues: newEntity,
            severity: "Medium",
            correlationId: correlationId
        );
    }

    /// <summary>
    /// Log an update action
    /// </summary>
    public async Task LogUpdateAsync<T>(string entityId, string entityName, T oldEntity, T newEntity, string? correlationId = null)
    {
        await LogActionInternalAsync(
            eventType: $"{typeof(T).Name}Updated",
            entityType: typeof(T).Name,
            entityId: entityId,
            entityName: entityName,
            action: "Update",
            oldValues: oldEntity,
            newValues: newEntity,
            severity: "Medium",
            correlationId: correlationId
        );
    }

    /// <summary>
    /// Log a delete action
    /// </summary>
    public async Task LogDeleteAsync<T>(string entityId, string entityName, T deletedEntity, string? correlationId = null)
    {
        await LogActionInternalAsync(
            eventType: $"{typeof(T).Name}Deleted",
            entityType: typeof(T).Name,
            entityId: entityId,
            entityName: entityName,
            action: "Delete",
            oldValues: deletedEntity,
            newValues: null,
            severity: "High",
            correlationId: correlationId
        );
    }

    /// <summary>
    /// Log a login action
    /// </summary>
    public async Task LogLoginAsync(Guid userId, string username, string email)
    {
        await LogActionInternalAsync(
            eventType: "UserLoggedIn",
            entityType: "User",
            entityId: userId.ToString(),
            entityName: username,
            action: "Login",
            oldValues: null,
            newValues: new { UserId = userId, Username = username, Email = email },
            severity: "Low",
            correlationId: null
        );
    }

    /// <summary>
    /// Log a logout action
    /// </summary>
    public async Task LogLogoutAsync(Guid userId, string username, string email)
    {
        await LogActionInternalAsync(
            eventType: "UserLoggedOut",
            entityType: "User",
            entityId: userId.ToString(),
            entityName: username,
            action: "Logout",
            oldValues: null,
            newValues: new { UserId = userId, Username = username, Email = email },
            severity: "Low",
            correlationId: null
        );
    }

    /// <summary>
    /// Log a custom action with specified severity
    /// </summary>
    public async Task LogCustomActionAsync(
        string eventType,
        string entityType,
        string entityId,
        string entityName,
        string action,
        object? oldValues,
        object? newValues,
        string severity = "Medium",
        string? correlationId = null)
    {
        await LogActionInternalAsync(
            eventType: eventType,
            entityType: entityType,
            entityId: entityId,
            entityName: entityName,
            action: action,
            oldValues: oldValues,
            newValues: newValues,
            severity: severity,
            correlationId: correlationId
        );
    }

    /// <summary>
    /// Internal method to perform the actual logging
    /// </summary>
    private async Task LogActionInternalAsync(
        string eventType,
        string entityType,
        string entityId,
        string entityName,
        string action,
        object? oldValues,
        object? newValues,
        string severity,
        string? correlationId)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                // If no HTTP context, skip logging (e.g., background jobs)
                return;
            }

            // Extract user information from claims
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var usernameClaim = httpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var emailClaim = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                // No valid user, skip logging
                return;
            }

            // Get IP address
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                ipAddress = httpContext.Request.Headers["X-Forwarded-For"].ToString();
            }

            // Get user agent
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

            // Determine source
            var source = "API";
            if (httpContext.Request.Headers.ContainsKey("Origin"))
            {
                var origin = httpContext.Request.Headers["Origin"].ToString();
                if (origin.Contains("localhost") || origin.Contains("127.0.0.1"))
                {
                    source = "Web";
                }
            }

            // Log the action
            await _auditLogService.LogActionAsync(
                eventType: eventType,
                entityType: entityType,
                entityId: entityId,
                entityName: entityName,
                action: action,
                oldValues: oldValues,
                newValues: newValues,
                userId: userId,
                username: usernameClaim ?? "Unknown",
                userEmail: emailClaim ?? "Unknown",
                ipAddress: ipAddress,
                userAgent: userAgent,
                source: source,
                severity: severity,
                correlationId: correlationId
            );
        }
        catch (Exception ex)
        {
            // Log the error but don't throw - audit logging should not break the application
            Console.WriteLine($"Error logging audit action: {ex.Message}");
        }
    }
}
