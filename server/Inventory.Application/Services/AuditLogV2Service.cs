using System.Text.Json;
using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Inventory.Application.Services;

public class AuditLogV2Service : IAuditLogV2Service
{
    private readonly IAuditLogV2Repository _repository;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditLogV2Service(
        IAuditLogV2Repository repository,
        IHttpContextAccessor httpContextAccessor)
    {
        _repository = repository;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(
        string actionType,
        string entityName,
        string entityId,
        object? oldValues,
        object? newValues,
        string message)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        // Extract user information from HttpContext claims
        var userId = httpContext?.User?.FindFirst("sub")?.Value 
                     ?? httpContext?.User?.FindFirst("userId")?.Value 
                     ?? "System";
        
        var userName = httpContext?.User?.FindFirst("name")?.Value 
                       ?? httpContext?.User?.FindFirst("username")?.Value 
                       ?? httpContext?.User?.Identity?.Name 
                       ?? "System";

        // Extract IP address from HttpContext
        var ipAddress = httpContext?.Connection?.RemoteIpAddress?.ToString() 
                        ?? httpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault() 
                        ?? "Unknown";

        // Serialize values to JSON
        string? oldValuesJson = oldValues != null 
            ? JsonSerializer.Serialize(oldValues, new JsonSerializerOptions { WriteIndented = false })
            : null;

        string? newValuesJson = newValues != null 
            ? JsonSerializer.Serialize(newValues, new JsonSerializerOptions { WriteIndented = false })
            : null;

        // Determine changed fields
        var changedFields = new List<string>();
        if (oldValues != null && newValues != null)
        {
            var oldDict = JsonSerializer.Deserialize<Dictionary<string, object>>(oldValuesJson!);
            var newDict = JsonSerializer.Deserialize<Dictionary<string, object>>(newValuesJson!);

            if (oldDict != null && newDict != null)
            {
                foreach (var key in newDict.Keys)
                {
                    if (oldDict.ContainsKey(key))
                    {
                        var oldVal = oldDict[key]?.ToString();
                        var newVal = newDict[key]?.ToString();
                        if (oldVal != newVal)
                        {
                            changedFields.Add(key);
                        }
                    }
                }
            }
        }

        string? changedFieldsJson = changedFields.Count > 0 
            ? JsonSerializer.Serialize(changedFields)
            : null;

        var auditLog = new AuditLogV2
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserName = userName,
            ActionType = actionType,
            EntityName = entityName,
            EntityId = entityId,
            Message = message,
            Timestamp = DateTime.UtcNow,
            OldValues = oldValuesJson,
            NewValues = newValuesJson,
            ChangedFields = changedFieldsJson,
            IpAddress = ipAddress
        };

        await _repository.AddAsync(auditLog);
    }

    public async Task<IEnumerable<AuditLogTimelineGroupDto>> GetTimelineAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var logs = await _repository.GetTimelineAsync(fromDate, toDate);

        var grouped = logs
            .GroupBy(l => l.Timestamp.Date)
            .Select(g => new AuditLogTimelineGroupDto
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                Logs = g.Select(l => new AuditLogTimelineDto
                {
                    Id = l.Id,
                    Message = l.Message,
                    Timestamp = l.Timestamp,
                    UserName = l.UserName,
                    EntityName = l.EntityName,
                    ActionType = l.ActionType
                }).ToList()
            })
            .OrderByDescending(g => g.Date)
            .ToList();

        return grouped;
    }

    public async Task<AuditLogDetailDto?> GetDetailAsync(Guid id)
    {
        var log = await _repository.GetByIdAsync(id);
        if (log == null) return null;

        object? oldValues = null;
        object? newValues = null;
        var changedFields = new List<string>();

        if (!string.IsNullOrEmpty(log.OldValues))
        {
            oldValues = JsonSerializer.Deserialize<Dictionary<string, object>>(log.OldValues);
        }

        if (!string.IsNullOrEmpty(log.NewValues))
        {
            newValues = JsonSerializer.Deserialize<Dictionary<string, object>>(log.NewValues);
        }

        if (!string.IsNullOrEmpty(log.ChangedFields))
        {
            changedFields = JsonSerializer.Deserialize<List<string>>(log.ChangedFields) ?? new List<string>();
        }

        return new AuditLogDetailDto
        {
            Id = log.Id,
            Message = log.Message,
            Timestamp = log.Timestamp,
            UserName = log.UserName,
            UserId = log.UserId,
            ActionType = log.ActionType,
            EntityName = log.EntityName,
            EntityId = log.EntityId,
            OldValues = oldValues,
            NewValues = newValues,
            ChangedFields = changedFields,
            IpAddress = log.IpAddress
        };
    }

    public async Task<IEnumerable<AuditLogV2Dto>> FilterAsync(
        DateTime? fromDate, 
        DateTime? toDate, 
        string? userId, 
        string? entityName)
    {
        var logs = await _repository.FilterAsync(fromDate, toDate, userId, entityName);

        return logs.Select(l => new AuditLogV2Dto
        {
            Id = l.Id,
            UserId = l.UserId,
            UserName = l.UserName,
            ActionType = l.ActionType,
            EntityName = l.EntityName,
            EntityId = l.EntityId,
            Message = l.Message,
            Timestamp = l.Timestamp,
            OldValues = l.OldValues,
            NewValues = l.NewValues,
            ChangedFields = l.ChangedFields,
            IpAddress = l.IpAddress
        }).ToList();
    }
}
