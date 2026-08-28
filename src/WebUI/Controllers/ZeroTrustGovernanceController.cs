#pragma warning disable
#pragma info disable
using System.Security.Claims;
using System.Threading.Tasks;
using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ZeroTrustGovernanceController : ControllerBase
{
    private readonly ISender _mediator;

    public ZeroTrustGovernanceController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("evaluate-access/{ideaId}")]
    public async Task<ActionResult<AbacAccessDecisionDto>> EvaluateAccess(
        int ideaId,
        [FromQuery] string? department,
        [FromQuery] string? clearance)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-anonymous";
        var decision = await _mediator.Send(new EvaluateIdeaAccessQuery(
            ideaId,
            userId,
            department ?? "General",
            clearance ?? "Internal"
        ));
        return Ok(decision);
    }

    [HttpGet("blind-review/idea/{ideaId}")]
    public async Task<ActionResult<AnonymizedIdeaDto>> GetAnonymizedIdea(int ideaId)
    {
        var anonymized = await _mediator.Send(new GetAnonymizedIdeaQuery(ideaId));
        return Ok(anonymized);
    }

    [HttpPost("blind-review/submit")]
    public async Task<ActionResult<BlindReviewResultDto>> SubmitBlindReview([FromBody] AnonymizedReviewCompletedCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
