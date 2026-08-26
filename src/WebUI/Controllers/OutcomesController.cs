using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutcomesController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IReputationService _reputationService;

    public OutcomesController(IApplicationDbContext context, IReputationService reputationService)
    {
        _context = context;
        _reputationService = reputationService;
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
    public async Task<ActionResult<IdeaOutcomeDto>> RecordOutcome([FromBody] RecordOutcomeDto dto)
    {
        var idea = await _context.Ideas.FindAsync(dto.IdeaId);
        if (idea == null) return NotFound("Idea not found");

        var outcome = new IdeaOutcome
        {
            IdeaId = dto.IdeaId,
            Title = dto.Title,
            Summary = dto.Summary,
            Type = dto.Type,
            ArtifactUrl = dto.ArtifactUrl,
            EstimatedCostSavings = dto.EstimatedCostSavings,
            RevenueGenerated = dto.RevenueGenerated,
            ImpactedUsersCount = dto.ImpactedUsersCount,
            EstimatedRoiPercent = dto.EstimatedRoiPercent,
            RetrospectiveNotes = dto.RetrospectiveNotes,
            KeyLearnings = dto.KeyLearnings,
            RealizedAt = DateTime.UtcNow
        };

        idea.MaturityStage = IdeaMaturityStage.Measured;

        _context.Outcomes.Add(outcome);

        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = dto.IdeaId,
            ActorName = "Outcome Evaluator",
            ActorRole = "Authority",
            ActionPerformed = "OutcomeRecorded",
            Details = $"Outcome '{outcome.Title}' ({outcome.Type}) recorded. Estimated ROI: {outcome.EstimatedRoiPercent}%.",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(default);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-admin";
        await _reputationService.RecordOutcomeAchievementAsync(userId, outcome.Id);

        return Ok(new IdeaOutcomeDto
        {
            Id = outcome.Id,
            IdeaId = outcome.IdeaId,
            Title = outcome.Title,
            Summary = outcome.Summary,
            Type = outcome.Type,
            EstimatedCostSavings = outcome.EstimatedCostSavings,
            RevenueGenerated = outcome.RevenueGenerated,
            ImpactedUsersCount = outcome.ImpactedUsersCount,
            EstimatedRoiPercent = outcome.EstimatedRoiPercent,
            RealizedAt = outcome.RealizedAt
        });
    }
}
