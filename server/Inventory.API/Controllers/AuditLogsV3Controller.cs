using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Inventory.API.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Inventory.API.Controllers;

/// <summary>
/// AuditLogsV3 Controller - Enterprise-level Jira-style audit logging API
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogsV3Controller : ControllerBase
{
    private readonly IAuditLogV3Service _auditLogService;
    private readonly IHubContext<AuditLogV3Hub> _hubContext;

    public AuditLogsV3Controller(
        IAuditLogV3Service auditLogService,
        IHubContext<AuditLogV3Hub> hubContext)
    {
        _auditLogService = auditLogService;
        _hubContext = hubContext;
    }

    /// <summary>
    /// Get paginated audit logs with filtering and sorting
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<PagedAuditLogV3Dto>> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? entityType = null,
        [FromQuery] string? action = null,
        [FromQuery] string? severity = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDescending = true)
    {
        var result = await _auditLogService.GetPagedAsync(
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

        return Ok(result);
    }

    /// <summary>
    /// Get audit log by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AuditLogV3Dto>> GetAuditLogById(Guid id)
    {
        var auditLog = await _auditLogService.GetByIdAsync(id);

        if (auditLog == null)
        {
            return NotFound(new { message = "Audit log not found" });
        }

        return Ok(auditLog);
    }

    /// <summary>
    /// Get audit logs for a specific entity
    /// </summary>
    [HttpGet("entity/{entityId}")]
    public async Task<ActionResult<IEnumerable<AuditLogV3Dto>>> GetAuditLogsByEntity(
        string entityId,
        [FromQuery] string? entityType = null)
    {
        var auditLogs = await _auditLogService.GetByEntityAsync(entityId, entityType);
        return Ok(auditLogs);
    }

    /// <summary>
    /// Get audit logs for a specific user
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<AuditLogV3Dto>>> GetAuditLogsByUser(
        Guid userId,
        [FromQuery] int top = 100)
    {
        var auditLogs = await _auditLogService.GetByUserAsync(userId, top);
        return Ok(auditLogs);
    }

    /// <summary>
    /// Get audit logs by correlation ID
    /// </summary>
    [HttpGet("correlation/{correlationId}")]
    public async Task<ActionResult<IEnumerable<AuditLogV3Dto>>> GetAuditLogsByCorrelation(string correlationId)
    {
        var auditLogs = await _auditLogService.GetByCorrelationIdAsync(correlationId);
        return Ok(auditLogs);
    }

    /// <summary>
    /// Get audit logs by severity
    /// </summary>
    [HttpGet("severity/{severity}")]
    public async Task<ActionResult<IEnumerable<AuditLogV3Dto>>> GetAuditLogsBySeverity(string severity)
    {
        var auditLogs = await _auditLogService.GetBySeverityAsync(severity);
        return Ok(auditLogs);
    }

    /// <summary>
    /// Create a new audit log entry
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AuditLogV3Dto>> CreateAuditLog([FromBody] CreateAuditLogV3Dto createDto)
    {
        var auditLog = await _auditLogService.CreateAsync(createDto);

        // Broadcast to SignalR clients
        await _hubContext.Clients.All.SendAsync("ReceiveAuditLogV3", auditLog);

        return CreatedAtAction(
            nameof(GetAuditLogById),
            new { id = auditLog.Id },
            auditLog
        );
    }

    /// <summary>
    /// Get current user's audit logs
    /// </summary>
    [HttpGet("my-logs")]
    public async Task<ActionResult<IEnumerable<AuditLogV3Dto>>> GetMyAuditLogs([FromQuery] int top = 100)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var auditLogs = await _auditLogService.GetByUserAsync(userId, top);
        return Ok(auditLogs);
    }

    /// <summary>
    /// Get statistics/summary of audit logs
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<object>> GetAuditLogStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var logs = await _auditLogService.GetPagedAsync(
            1, 
            10000, 
            startDate: startDate, 
            endDate: endDate
        );

        var stats = new
        {
            TotalLogs = logs.TotalCount,
            BySeverity = logs.Items.GroupBy(l => l.Severity)
                .Select(g => new { Severity = g.Key, Count = g.Count() }),
            ByAction = logs.Items.GroupBy(l => l.Action)
                .Select(g => new { Action = g.Key, Count = g.Count() }),
            ByEntityType = logs.Items.GroupBy(l => l.EntityType)
                .Select(g => new { EntityType = g.Key, Count = g.Count() }),
            TopUsers = logs.Items.GroupBy(l => l.Username)
                .Select(g => new { Username = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
        };

        return Ok(stats);
    }
}
