#pragma warning disable
#pragma info disable
using System.Collections.Generic;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionPlaybookController : ControllerBase
{
    private readonly ISender _mediator;

    public SessionPlaybookController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("templates")]
    public async Task<ActionResult<List<WorkshopPlaybookDto>>> GetTemplates([FromQuery] string? formatId)
    {
        var playbooks = await _mediator.Send(new GetPlaybookTemplatesQuery(formatId));
        return Ok(playbooks);
    }

    [HttpPost("advance")]
    public async Task<ActionResult<PlaybookPhaseProgressDto>> AdvancePhase([FromBody] AdvancePlaybookPhaseCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
