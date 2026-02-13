using Inventory.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/auditlogs/v2")]
[Authorize]
public class AuditLogsV2Controller : ControllerBase
{
    private readonly IAuditLogV2Service _auditLogService;

    public AuditLogsV2Controller(IAuditLogV2Service auditLogService)
    {
        _auditLogService = auditLogService;
    }

    /// <summary>
    /// Get timeline view of audit logs grouped by date
    /// </summary>
    [HttpGet("timeline")]
    public async Task<IActionResult> GetTimeline(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var timeline = await _auditLogService.GetTimelineAsync(fromDate, toDate);
        return Ok(timeline);
    }

    /// <summary>
    /// Get detailed audit log by ID with before/after comparison
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(Guid id)
    {
        var detail = await _auditLogService.GetDetailAsync(id);
        if (detail == null)
            return NotFound(new { message = "Audit log not found" });

        return Ok(detail);
    }

    /// <summary>
    /// Filter audit logs by multiple criteria
    /// </summary>
    [HttpGet("filter")]
    public async Task<IActionResult> Filter(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] string? userId = null,
        [FromQuery] string? entityName = null)
    {
        var logs = await _auditLogService.FilterAsync(fromDate, toDate, userId, entityName);
        return Ok(logs);
    }
}
