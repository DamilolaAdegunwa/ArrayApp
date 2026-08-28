#pragma warning disable
#pragma info disable
using System.Security.Claims;
using System.Threading.Tasks;
using ArrayApp.Application.Ideas.Commands;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InnovationEconomyController : ControllerBase
{
    private readonly ISender _mediator;

    public InnovationEconomyController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("vote/quadratic")]
    public async Task<ActionResult<QuadraticVoteResultDto>> CastQuadraticVote([FromBody] CastQuadraticVoteCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("prediction/place")]
    public async Task<ActionResult<IdeaPredictionResultDto>> PlacePrediction([FromBody] PlaceIdeaPredictionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("bounty/attach")]
    public async Task<ActionResult<BountyAttachmentResultDto>> AttachBounty([FromBody] AttachBountyCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
