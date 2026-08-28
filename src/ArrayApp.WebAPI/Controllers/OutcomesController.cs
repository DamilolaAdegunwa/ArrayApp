#pragma warning disable
#pragma info disable
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using ArrayApp.Domain.Entities.IdeaAggregate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutcomesController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IReputationService _reputationService;
    private readonly ISender _mediator;

    public OutcomesController(IApplicationDbContext context, IReputationService reputationService, ISender mediator)
    {
        _context = context;
        _reputationService = reputationService;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<List<IdeaOutcomeDto>>> GetOutcomes([FromQuery] int? ideaId)
    {
        var query = _context.Outcomes.AsNoTracking();
        if (ideaId.HasValue)
        {
            query = query.Where(o => o.IdeaId == ideaId.Value);
        }

        var outcomes = await query.OrderByDescending(o => o.RealizedAt).ToListAsync();

        return Ok(outcomes.Select(o => new IdeaOutcomeDto
        {
            Id = o.Id,
            IdeaId = o.IdeaId,
            Title = o.Title,
            Summary = o.Summary,
            Type = o.Type,
            ArtifactUrl = o.ArtifactUrl,
            EstimatedCostSavings = o.EstimatedCostSavings,
            RevenueGenerated = o.RevenueGenerated,
            ImpactedUsersCount = o.ImpactedUsersCount,
            EstimatedRoiPercent = o.EstimatedRoiPercent,
            RetrospectiveNotes = o.RetrospectiveNotes,
            KeyLearnings = o.KeyLearnings,
            RealizedAt = o.RealizedAt
        }).ToList());
    }

    [HttpPost]
    public async Task<ActionResult<IdeaOutcomeDto>> RecordOutcome([FromBody] RecordIdeaOutcomeCommand command)
    {
        var outcome = await _mediator.Send(command);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-admin";
        await _reputationService.RecordOutcomeAchievementAsync(userId, outcome.Id);
        return Ok(outcome);
    }

    [HttpGet("analytics")]
    public async Task<ActionResult<InnovationPipelineAnalyticsDto>> GetPipelineAnalytics()
    {
        var analytics = await _mediator.Send(new GetExecutivePipelineAnalyticsQuery());
        return Ok(analytics);
    }

    [HttpGet("risk-matrix")]
    public async Task<ActionResult<PortfolioRiskMatrixDto>> GetRiskMatrix()
    {
        var matrix = await _mediator.Send(new GetPortfolioRiskMatrixQuery());
        return Ok(matrix);
    }
}
