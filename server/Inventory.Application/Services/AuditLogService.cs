using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.Domain.Interfaces;

namespace Inventory.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditLogService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<AuditLogDto>> GetAllLogsAsync()
    {
        var logs = await _unitOfWork.AuditLogs.GetAllAsync();
        return logs.Select(l => new AuditLogDto
        {
            Id = l.Id,
            UserId = l.UserId,
            Username = l.User?.Username ?? "Unknown",
            Action = l.Action,
            EntityName = l.EntityName,
            EntityId = l.EntityId,
            OldValues = l.OldValues,
            NewValues = l.NewValues,
            Timestamp = l.Timestamp,
            IpAddress = l.IpAddress
        }).OrderByDescending(l => l.Timestamp);
    }

    public async Task<IEnumerable<AuditLogDto>> GetLogsByUserIdAsync(Guid userId)
    {
        var logs = await _unitOfWork.AuditLogs.GetLogsByUserIdAsync(userId);
        return logs.Select(l => new AuditLogDto
        {
            Id = l.Id,
            UserId = l.UserId,
            Username = l.User?.Username ?? "Unknown",
            Action = l.Action,
            EntityName = l.EntityName,
            EntityId = l.EntityId,
            OldValues = l.OldValues,
            NewValues = l.NewValues,
            Timestamp = l.Timestamp,
            IpAddress = l.IpAddress
        });
    }

    public async Task<IEnumerable<AuditLogDto>> GetLogsByActionAsync(string action)
    {
        var logs = await _unitOfWork.AuditLogs.GetLogsByActionAsync(action);
        return logs.Select(l => new AuditLogDto
        {
            Id = l.Id,
            UserId = l.UserId,
            Username = l.User?.Username ?? "Unknown",
            Action = l.Action,
            EntityName = l.EntityName,
            EntityId = l.EntityId,
            OldValues = l.OldValues,
            NewValues = l.NewValues,
            Timestamp = l.Timestamp,
            IpAddress = l.IpAddress
        });
    }
}
