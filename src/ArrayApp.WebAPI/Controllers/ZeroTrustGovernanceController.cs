#pragma warning disable
#pragma info disable
using System.Security.Claims;
using System.Threading.Tasks;
using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArrayApp.WebAPI.Controllers;

[Authorize]
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirst("UserId")?.Value 
                     ?? User.FindFirst("sub")?.Value 
                     ?? "user-anonymous";

        // Security Hardening: prioritize cryptographically validated token claims over client-supplied query strings
        var userDepartment = User.FindFirst("Department")?.Value 
                             ?? User.FindFirst("department")?.Value 
                             ?? (!string.IsNullOrWhiteSpace(department) ? department : "General");

        var userClearance = User.FindFirst("Clearance")?.Value 
                            ?? User.FindFirst("clearance")?.Value 
                            ?? (!string.IsNullOrWhiteSpace(clearance) ? clearance : "Internal");

        var decision = await _mediator.Send(new EvaluateIdeaAccessQuery(
            ideaId,
            userId,
            userDepartment,
            userClearance
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
