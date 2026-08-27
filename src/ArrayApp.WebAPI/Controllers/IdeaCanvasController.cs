#pragma warning disable
#pragma info disable
using System.Collections.Generic;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Models;
using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IdeaCanvasController : ControllerBase
{
    private readonly ISender _mediator;

    public IdeaCanvasController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{ideaId}")]
    public async Task<ActionResult<List<IdeaCanvasNodeDto>>> GetNodes(int ideaId, [FromQuery] int? sessionId)
    {
        var nodes = await _mediator.Send(new GetCanvasNodesQuery(ideaId, sessionId));
        return Ok(nodes);
    }

    [HttpPost("node")]
    public async Task<ActionResult<IdeaCanvasNodeDto>> SaveNode([FromBody] SaveCanvasNodeCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("node/{nodeId}/vote")]
    public async Task<ActionResult<int>> VoteNode(int nodeId, [FromQuery] int ideaId, [FromQuery] int increment = 1)
    {
        var votes = await _mediator.Send(new VoteCanvasNodeCommand(ideaId, nodeId, increment));
        return Ok(votes);
    }

    [HttpPost("{ideaId}/cluster")]
    public async Task<ActionResult<List<IdeaCanvasNodeDto>>> ClusterNodes(int ideaId, [FromQuery] int? sessionId)
    {
        var clustered = await _mediator.Send(new AutoClusterCanvasNodesCommand(ideaId, sessionId));
        return Ok(clustered);
    }
}
