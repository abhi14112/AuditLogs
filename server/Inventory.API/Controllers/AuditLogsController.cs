using Inventory.Application.DTOs;
using Inventory.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogsController : ControllerBase
{
    private readonly IAuditLogService _auditLogService;

    public AuditLogsController(IAuditLogService auditLogService)
    {
        _auditLogService = auditLogService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAllLogs()
    {
        var logs = await _auditLogService.GetAllLogsAsync();
        return Ok(logs);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetLogsByUserId(Guid userId)
    {
        var logs = await _auditLogService.GetLogsByUserIdAsync(userId);
        return Ok(logs);
    }

    [HttpGet("action/{action}")]
    public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetLogsByAction(string action)
    {
        var logs = await _auditLogService.GetLogsByActionAsync(action);
        return Ok(logs);
    }
}
