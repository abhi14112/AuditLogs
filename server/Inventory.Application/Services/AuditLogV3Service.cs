using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using System.Text.Json;

namespace Inventory.Application.Services;

/// <summary>
/// Service implementation for AuditLogV3 - Enterprise-level Jira-style audit logging
/// </summary>
public class AuditLogV3Service : IAuditLogV3Service
{
    private readonly IAuditLogV3Repository _repository;
    private readonly IAuditLogV3Notifier? _notifier;

    public AuditLogV3Service(IAuditLogV3Repository repository, IAuditLogV3Notifier? notifier = null)
    {
        _repository = repository;
        _notifier = notifier;
    }

    public async Task<AuditLogV3Dto> CreateAsync(CreateAuditLogV3Dto createDto)
    {
        var auditLog = new AuditLogV3
        {
            Id = Guid.NewGuid(),
            EventType = createDto.EventType,
            EntityType = createDto.EntityType,
            EntityId = createDto.EntityId,
            EntityName = createDto.EntityName,
            Action = createDto.Action,
            Description = createDto.Description,
            OldValues = createDto.OldValues,
            NewValues = createDto.NewValues,
            ChangesSummary = createDto.ChangesSummary,
            UserId = createDto.UserId,
            Username = createDto.Username,
            UserEmail = createDto.UserEmail,
            IpAddress = createDto.IpAddress,
            UserAgent = createDto.UserAgent,
            Source = createDto.Source,
            CreatedAt = DateTime.UtcNow,
            CorrelationId = createDto.CorrelationId ?? Guid.NewGuid().ToString(),
            Status = createDto.Status,
            Severity = createDto.Severity
        };

        var created = await _repository.CreateAsync(auditLog);
        var dto = MapToDto(created);
        
        // Send real-time notification if notifier is available
        if (_notifier != null)
        {
            await _notifier.NotifyAuditLogCreatedAsync(dto);
        }
        
        return dto;
    }

    public async Task<AuditLogV3Dto?> GetByIdAsync(Guid id)
    {
        var auditLog = await _repository.GetByIdAsync(id);
        return auditLog != null ? MapToDto(auditLog) : null;
    }

    public async Task<IEnumerable<AuditLogV3Dto>> GetAllAsync()
    {
        var auditLogs = await _repository.GetAllAsync();
        return auditLogs.Select(MapToDto);
    }

    public async Task<PagedAuditLogV3Dto> GetPagedAsync(
        int page = 1,
        int pageSize = 20,
        string? searchTerm = null,
        string? entityType = null,
        string? action = null,
        string? severity = null,
        Guid? userId = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        string? sortBy = null,
        bool sortDescending = true)
    {
        var (items, totalCount) = await _repository.GetPagedAsync(
            page,
            pageSize,
            searchTerm,
            entityType,
            action,
            severity,
            userId,
            startDate,
            endDate,
            sortBy,
            sortDescending
        );

        return new PagedAuditLogV3Dto
        {
            Items = items.Select(MapToDto),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<IEnumerable<AuditLogV3Dto>> GetByEntityAsync(string entityId, string? entityType = null)
    {
        var auditLogs = await _repository.GetByEntityAsync(entityId, entityType);
        return auditLogs.Select(MapToDto);
    }

    public async Task<IEnumerable<AuditLogV3Dto>> GetByUserAsync(Guid userId, int top = 100)
    {
        var auditLogs = await _repository.GetByUserAsync(userId, top);
        return auditLogs.Select(MapToDto);
    }

    public async Task<IEnumerable<AuditLogV3Dto>> GetByCorrelationIdAsync(string correlationId)
    {
        var auditLogs = await _repository.GetByCorrelationIdAsync(correlationId);
        return auditLogs.Select(MapToDto);
    }

    public async Task<IEnumerable<AuditLogV3Dto>> GetBySeverityAsync(string severity)
    {
        var auditLogs = await _repository.GetBySeverityAsync(severity);
        return auditLogs.Select(MapToDto);
    }

    public async Task LogActionAsync(
        string eventType,
        string entityType,
        string entityId,
        string entityName,
        string action,
        object? oldValues,
        object? newValues,
        Guid userId,
        string username,
        string userEmail,
        string ipAddress,
        string? userAgent = null,
        string source = "Web",
        string severity = "Medium",
        string? correlationId = null)
    {
        // Generate changes summary
        var changesSummary = GenerateChangesSummary(oldValues, newValues);

        var createDto = new CreateAuditLogV3Dto
        {
            EventType = eventType,
            EntityType = entityType,
            EntityId = entityId,
            EntityName = entityName,
            Action = action,
            Description = $"{username} {action.ToLower()}d {entityType} '{entityName}'",
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            ChangesSummary = changesSummary,
            UserId = userId,
            Username = username,
            UserEmail = userEmail,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            Source = source,
            Status = "Success",
            Severity = severity,
            CorrelationId = correlationId
        };

        await CreateAsync(createDto);
    }

    private string? GenerateChangesSummary(object? oldValues, object? newValues)
    {
        if (oldValues == null || newValues == null)
            return null;

        try
        {
            var oldDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                JsonSerializer.Serialize(oldValues));
            var newDict = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(
                JsonSerializer.Serialize(newValues));

            if (oldDict == null || newDict == null)
                return null;

            var changes = new List<string>();

            foreach (var key in newDict.Keys)
            {
                if (oldDict.ContainsKey(key))
                {
                    var oldVal = oldDict[key].ToString();
                    var newVal = newDict[key].ToString();

                    if (oldVal != newVal)
                    {
                        changes.Add($"{key}: {oldVal} → {newVal}");
                    }
                }
            }

            return changes.Any() ? string.Join(", ", changes) : null;
        }
        catch
        {
            return null;
        }
    }

    private AuditLogV3Dto MapToDto(AuditLogV3 auditLog)
    {
        return new AuditLogV3Dto
        {
            Id = auditLog.Id,
            EventType = auditLog.EventType,
            EntityType = auditLog.EntityType,
            EntityId = auditLog.EntityId,
            EntityName = auditLog.EntityName,
            Action = auditLog.Action,
            Description = auditLog.Description,
            OldValues = auditLog.OldValues,
            NewValues = auditLog.NewValues,
            ChangesSummary = auditLog.ChangesSummary,
            UserId = auditLog.UserId,
            Username = auditLog.Username,
            UserEmail = auditLog.UserEmail,
            IpAddress = auditLog.IpAddress,
            UserAgent = auditLog.UserAgent,
            Source = auditLog.Source,
            CreatedAt = auditLog.CreatedAt,
            CorrelationId = auditLog.CorrelationId,
            Status = auditLog.Status,
            Severity = auditLog.Severity
        };
    }
}
