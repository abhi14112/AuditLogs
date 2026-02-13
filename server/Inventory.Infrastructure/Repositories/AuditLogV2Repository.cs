using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

public class AuditLogV2Repository : IAuditLogV2Repository
{
    private readonly ApplicationDbContext _context;

    public AuditLogV2Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLogV2> AddAsync(AuditLogV2 auditLog)
    {
        _context.AuditLogsV2.Add(auditLog);
        await _context.SaveChangesAsync();
        return auditLog;
    }

    public async Task<AuditLogV2?> GetByIdAsync(Guid id)
    {
        return await _context.AuditLogsV2
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<AuditLogV2>> GetAllAsync()
    {
        return await _context.AuditLogsV2
            .AsNoTracking()
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLogV2>> GetTimelineAsync(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.AuditLogsV2.AsNoTracking();

        if (fromDate.HasValue)
            query = query.Where(a => a.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.Timestamp <= toDate.Value);

        return await query
            .OrderByDescending(a => a.Timestamp)
            .Select(a => new AuditLogV2
            {
                Id = a.Id,
                Message = a.Message,
                Timestamp = a.Timestamp,
                UserName = a.UserName,
                EntityName = a.EntityName,
                ActionType = a.ActionType,
                UserId = a.UserId,
                EntityId = a.EntityId,
                IpAddress = a.IpAddress
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLogV2>> FilterAsync(
        DateTime? fromDate, 
        DateTime? toDate, 
        string? userId, 
        string? entityName)
    {
        var query = _context.AuditLogsV2.AsNoTracking();

        if (fromDate.HasValue)
            query = query.Where(a => a.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.Timestamp <= toDate.Value);

        if (!string.IsNullOrWhiteSpace(userId))
            query = query.Where(a => a.UserId == userId);

        if (!string.IsNullOrWhiteSpace(entityName))
            query = query.Where(a => a.EntityName == entityName);

        return await query
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }
}
