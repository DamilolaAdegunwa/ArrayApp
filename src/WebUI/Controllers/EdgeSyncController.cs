#pragma warning disable
#pragma info disable
using System.Threading.Tasks;
using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EdgeSyncController : ControllerBase
{
    private readonly ISender _mediator;

    public EdgeSyncController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("snapshot/{ideaId}")]
    public async Task<ActionResult<CrdtIdeaStateSnapshotDto>> GetSnapshot(int ideaId)
    {
        var snapshot = await _mediator.Send(new GetCrdtStateQuery(ideaId));
        return Ok(snapshot);
    }

    [HttpPost("reconcile")]
    public async Task<ActionResult<CrdtReconciliationResultDto>> ReconcileOperations([FromBody] ReconcileCrdtOperationsCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
