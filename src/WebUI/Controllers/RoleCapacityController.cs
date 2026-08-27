#pragma warning disable
#pragma info disable
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using ArrayApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleCapacityController : ControllerBase
{
    private readonly ISender _mediator;

    public RoleCapacityController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("execute")]
    public async Task<ActionResult<RoleActionResultDto>> ExecuteRoleAction([FromBody] ExecuteRoleActionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("history/{ideaId}")]
    public async Task<ActionResult<List<RoleActionHistoryDto>>> GetActionHistory(int ideaId)
    {
        var history = await _mediator.Send(new GetRoleActionHistoryQuery(ideaId));
        return Ok(history);
    }
}
