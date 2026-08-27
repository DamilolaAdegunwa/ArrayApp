using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;

// =========================================================================================================
// [NEW CORE ARCHITECTURAL ADDITION]: RoleCapacityController
// REST API exposing specialized 10-role action execution, reputation awarding, and history audit logs
// =========================================================================================================
[ApiController]
[Route("api/[controller]")]
public class RoleCapacityController : ControllerBase
{
    private readonly IRoleCapacityService _roleCapacityService;

    public RoleCapacityController(IRoleCapacityService roleCapacityService)
    {
        _roleCapacityService = roleCapacityService;
    }

    [HttpPost("execute-action")]
    public async Task<ActionResult<RoleActionResultDto>> ExecuteRoleAction([FromBody] ExecuteRoleActionRequestDto request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-elena";
        var result = await _roleCapacityService.ExecuteRoleActionAsync(request, userId);
        return Ok(result);
    }

    [HttpGet("history/{ideaId}")]
    public async Task<ActionResult<List<RoleActionHistoryDto>>> GetRoleActionHistory(int ideaId)
    {
        var history = await _roleCapacityService.GetRoleActionHistoryAsync(ideaId);
        return Ok(history);
    }
}
