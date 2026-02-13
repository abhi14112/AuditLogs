using Inventory.Domain.Entities;
using Inventory.Domain.Interfaces;
using Inventory.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for AuditLogV3 with advanced querying capabilities
/// </summary>
public class AuditLogV3Repository : IAuditLogV3Repository
{
    private readonly ApplicationDbContext _context;

    public AuditLogV3Repository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLogV3> CreateAsync(AuditLogV3 auditLog)
    {
        _context.AuditLogsV3.Add(auditLog);
        await _context.SaveChangesAsync();
        return auditLog;
    }

    public async Task<AuditLogV3?> GetByIdAsync(Guid id)
    {
        return await _context.AuditLogsV3.FindAsync(id);
    }

    public async Task<IEnumerable<AuditLogV3>> GetAllAsync()
    {
        return await _context.AuditLogsV3
            .OrderByDescending(a => a.CreatedAt)
            .Take(1000) // Limit to prevent performance issues
            .ToListAsync();
    }

    public async Task<(IEnumerable<AuditLogV3> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
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
        var query = _context.AuditLogsV3.AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.Description.Contains(searchTerm) ||
                a.EntityName.Contains(searchTerm) ||
                a.Username.Contains(searchTerm) ||
                a.EntityId.Contains(searchTerm)
            );
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(a => a.EntityType == entityType);
        }

        if (!string.IsNullOrWhiteSpace(action))
        {
            query = query.Where(a => a.Action == action);
        }

        if (!string.IsNullOrWhiteSpace(severity))
        {
            query = query.Where(a => a.Severity == severity);
        }

        if (userId.HasValue)
        {
            query = query.Where(a => a.UserId == userId.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= endDate.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply sorting
        query = ApplySorting(query, sortBy, sortDescending);

        // Apply pagination
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<AuditLogV3>> GetByEntityAsync(string entityId, string? entityType = null)
    {
        var query = _context.AuditLogsV3
            .Where(a => a.EntityId == entityId);

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            query = query.Where(a => a.EntityType == entityType);
        }

        return await query
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLogV3>> GetByUserAsync(Guid userId, int top = 100)
    {
        return await _context.AuditLogsV3
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(top)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLogV3>> GetByCorrelationIdAsync(string correlationId)
    {
        return await _context.AuditLogsV3
            .Where(a => a.CorrelationId == correlationId)
            .OrderBy(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLogV3>> GetBySeverityAsync(string severity)
    {
        return await _context.AuditLogsV3
            .Where(a => a.Severity == severity)
            .OrderByDescending(a => a.CreatedAt)
            .Take(500)
            .ToListAsync();
    }

    private IQueryable<AuditLogV3> ApplySorting(IQueryable<AuditLogV3> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLower() switch
        {
            "eventtype" => sortDescending
                ? query.OrderByDescending(a => a.EventType)
                : query.OrderBy(a => a.EventType),
            "entitytype" => sortDescending
                ? query.OrderByDescending(a => a.EntityType)
                : query.OrderBy(a => a.EntityType),
            "action" => sortDescending
                ? query.OrderByDescending(a => a.Action)
                : query.OrderBy(a => a.Action),
            "username" => sortDescending
                ? query.OrderByDescending(a => a.Username)
                : query.OrderBy(a => a.Username),
            "severity" => sortDescending
                ? query.OrderByDescending(a => a.Severity)
                : query.OrderBy(a => a.Severity),
            "createdat" or _ => sortDescending
                ? query.OrderByDescending(a => a.CreatedAt)
                : query.OrderBy(a => a.CreatedAt)
        };
    }
}
