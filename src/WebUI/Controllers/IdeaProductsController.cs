using System.Collections.Generic;
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
public class IdeaProductsController : ControllerBase
{
    private readonly IIdeaProductService _ideaProductService;
    private readonly IApplicationDbContext _context;
    private readonly IReputationService _reputationService;

    public IdeaProductsController(IIdeaProductService ideaProductService, IApplicationDbContext context, IReputationService reputationService)
    {
        _ideaProductService = ideaProductService;
        _context = context;
        _reputationService = reputationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<IdeaProductDto>>> GetIdeas([FromQuery] IdeaMaturityStage? stage, [FromQuery] int? categoryId, [FromQuery] string? search)
    {
        var ideas = await _ideaProductService.GetIdeasAsync(stage, categoryId, search);
        return Ok(ideas);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<IdeaProductDto>> GetIdeaById(int id)
    {
        var idea = await _ideaProductService.GetIdeaByIdAsync(id);
        if (idea == null) return NotFound();
        return Ok(idea);
    }

    [HttpPost]
    public async Task<ActionResult<IdeaProductDto>> CreateIdea([FromBody] CreateIdeaProductDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-admin";
        var created = await _ideaProductService.CreateIdeaAsync(dto, userId);
        return CreatedAtAction(nameof(GetIdeaById), new { id = created.Id }, created);
    }

    [HttpPut("{id}/maturity")]
    public async Task<IActionResult> UpdateMaturityStage(int id, [FromBody] UpdateIdeaMaturityStageDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-admin";
        var success = await _ideaProductService.UpdateMaturityStageAsync(id, dto.NewStage, dto.Rationale, userId);
        if (!success) return NotFound();
        return NoContent();
    }

    [HttpPost("{id}/fork")]
    public async Task<ActionResult<IdeaProductDto>> ForkIdea(int id, [FromBody] ForkIdeaDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-admin";
        var forked = await _ideaProductService.ForkIdeaAsync(id, dto, userId);
        return CreatedAtAction(nameof(GetIdeaById), new { id = forked.Id }, forked);
    }

    [HttpGet("graph")]
    public async Task<ActionResult<IdeaGraphDto>> GetIdeaGraph([FromQuery] int? focusIdeaId)
    {
        var graph = await _ideaProductService.GetIdeaGraphAsync(focusIdeaId);
        return Ok(graph);
    }

    [HttpGet("analytics")]
    public async Task<ActionResult<InnovationPipelineAnalyticsDto>> GetPipelineAnalytics()
    {
        var analytics = await _ideaProductService.GetPipelineAnalyticsAsync();
        return Ok(analytics);
    }

    [HttpPost("{id}/vote")]
    public async Task<IActionResult> VoteIdea(int id, [FromQuery] bool isUpvote)
    {
        var idea = await _context.Ideas.FindAsync(id);
        if (idea == null) return NotFound();

        if (isUpvote) idea.Upvotes++;
        else idea.Downvotes++;

        await _context.SaveChangesAsync(default);
        return Ok(new { idea.Id, idea.Upvotes, idea.Downvotes });
    }

    [HttpPost("{id}/subscribe")]
    public async Task<IActionResult> SubscribeToIdea(int id, [FromBody] JoinSessionDto dto)
    {
        var idea = await _context.Ideas.Include(i => i.Subscriptions).FirstOrDefaultAsync(i => i.Id == id);
        if (idea == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-demo";
        var sub = idea.Subscriptions.FirstOrDefault(s => s.UserId == userId);

        if (sub == null)
        {
            sub = new IdeaSubscription
            {
                IdeaId = id,
                UserId = userId,
                Role = dto.Role,
                ContributionsCount = 1
            };
            idea.Subscriptions.Add(sub);
        }
        else
        {
            sub.Role = dto.Role;
        }

        await _reputationService.RecordIdeaContributionAsync(userId, id, dto.Role);
        await _context.SaveChangesAsync(default);

        return Ok(new { success = true, role = dto.Role.ToString() });
    }

    [HttpPost("{id}/score")]
    public async Task<IActionResult> SubmitScore(int id, [FromBody] SubmitIdeaScoreDto dto)
    {
        var idea = await _context.Ideas.FindAsync(id);
        if (idea == null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-demo";
        await _reputationService.AwardPointsAsync(userId, 20, $"Submitted peer review & multidimensional evaluation on Idea #{id}");

        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = id,
            ActorName = User.Identity?.Name ?? "Peer Reviewer",
            ActorRole = "Professional",
            ActionPerformed = "MultidimensionalScoreSubmitted",
            Details = $"Impact: {dto.ImpactScore}/10, Confidence: {dto.ConfidenceScore}/10, Ease: {dto.EaseScore}/10. Feedback: {dto.ReviewFeedback}",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(default);
        return Ok(new { success = true, ideaId = id, calculatedIceScore = Math.Round((dto.ImpactScore * dto.ConfidenceScore * dto.EaseScore) / 10.0, 1) });
    }

    #region Knowledge Gaps Endpoints
    [HttpGet("gaps/all")]
    public async Task<ActionResult<List<KnowledgeGapDto>>> GetAllCrowdsourcedGaps([FromQuery] string? domain, [FromQuery] KnowledgeGapStatus? status)
    {
        var query = _context.KnowledgeGaps.Include(g => g.Idea).AsNoTracking();
        if (!string.IsNullOrWhiteSpace(domain))
        {
            query = query.Where(g => g.DomainArea.ToLower().Contains(domain.ToLower()));
        }
        if (status.HasValue)
        {
            query = query.Where(g => g.Status == status.Value);
        }

        var gaps = await query.OrderByDescending(g => g.Priority)
            .Select(g => new KnowledgeGapDto
            {
                Id = g.Id,
                IdeaId = g.IdeaId,
                Title = g.Title,
                Description = g.Description,
                DomainArea = g.DomainArea,
                Priority = g.Priority,
                Status = g.Status,
                ResolutionDetails = g.ResolutionDetails,
                SupportingEvidenceUrl = g.SupportingEvidenceUrl,
                ResolvedAt = g.ResolvedAt
            })
            .ToListAsync();

        return Ok(gaps);
    }

    [HttpGet("{id}/gaps")]
    public async Task<ActionResult<List<KnowledgeGapDto>>> GetKnowledgeGaps(int id)
    {
        var gaps = await _context.KnowledgeGaps
            .Where(g => g.IdeaId == id)
            .OrderByDescending(g => g.Priority)
            .Select(g => new KnowledgeGapDto
            {
                Id = g.Id,
                IdeaId = g.IdeaId,
                Title = g.Title,
                Description = g.Description,
                DomainArea = g.DomainArea,
                Priority = g.Priority,
                Status = g.Status,
                ResolutionDetails = g.ResolutionDetails,
                ResolvedAt = g.ResolvedAt
            })
            .ToListAsync();

        return Ok(gaps);
    }

    [HttpPost("{id}/gaps")]
    public async Task<ActionResult<KnowledgeGapDto>> AddKnowledgeGap(int id, [FromBody] CreateKnowledgeGapDto dto)
    {
        var idea = await _context.Ideas.FindAsync(id);
        if (idea == null) return NotFound();

        var gap = new KnowledgeGap
        {
            IdeaId = id,
            Title = dto.Title,
            Description = dto.Description,
            DomainArea = dto.DomainArea,
            Priority = dto.Priority,
            Status = KnowledgeGapStatus.Open
        };

        _context.KnowledgeGaps.Add(gap);
        await _context.SaveChangesAsync(default);

        return Ok(new KnowledgeGapDto
        {
            Id = gap.Id,
            IdeaId = gap.IdeaId,
            Title = gap.Title,
            Description = gap.Description,
            DomainArea = gap.DomainArea,
            Priority = gap.Priority,
            Status = gap.Status
        });
    }

    [HttpPut("gaps/{gapId}/resolve")]
    public async Task<IActionResult> ResolveKnowledgeGap(int gapId, [FromBody] ResolveKnowledgeGapDto dto)
    {
        var gap = await _context.KnowledgeGaps.FindAsync(gapId);
        if (gap == null) return NotFound();

        gap.Status = KnowledgeGapStatus.Resolved;
        gap.ResolutionDetails = dto.ResolutionDetails;
        gap.SupportingEvidenceUrl = dto.SupportingEvidenceUrl;
        gap.ResolvedAt = System.DateTime.UtcNow;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-admin";
        await _reputationService.AwardPointsAsync(userId, 35, "Resolved a critical knowledge gap");

        await _context.SaveChangesAsync(default);
        return NoContent();
    }
    #endregion

    #region Experiments Endpoints
    [HttpGet("{id}/experiments")]
    public async Task<ActionResult<List<IdeaExperimentDto>>> GetExperiments(int id)
    {
        var experiments = await _context.Experiments
            .Where(e => e.IdeaId == id)
            .Select(e => new IdeaExperimentDto
            {
                Id = e.Id,
                IdeaId = e.IdeaId,
                HypothesisId = e.HypothesisId,
                Title = e.Title,
                Description = e.Description,
                Protocol = e.Protocol,
                RequiredResources = e.RequiredResources,
                ExpectedMetric = e.ExpectedMetric,
                ActualResult = e.ActualResult,
                Learnings = e.Learnings,
                Status = e.Status,
                StartedAt = e.StartedAt,
                CompletedAt = e.CompletedAt
            })
            .ToListAsync();

        return Ok(experiments);
    }

    [HttpPost("{id}/experiments")]
    public async Task<ActionResult<IdeaExperimentDto>> CreateExperiment(int id, [FromBody] CreateExperimentDto dto)
    {
        var idea = await _context.Ideas.FindAsync(id);
        if (idea == null) return NotFound();

        var experiment = new IdeaExperiment
        {
            IdeaId = id,
            HypothesisId = dto.HypothesisId,
            Title = dto.Title,
            Description = dto.Description,
            Protocol = dto.Protocol,
            RequiredResources = dto.RequiredResources,
            ExpectedMetric = dto.ExpectedMetric,
            Status = ExperimentStatus.Running,
            StartedAt = System.DateTime.UtcNow
        };

        _context.Experiments.Add(experiment);
        await _context.SaveChangesAsync(default);

        return Ok(new IdeaExperimentDto
        {
            Id = experiment.Id,
            IdeaId = experiment.IdeaId,
            Title = experiment.Title,
            Description = experiment.Description,
            Protocol = experiment.Protocol,
            ExpectedMetric = experiment.ExpectedMetric,
            Status = experiment.Status
        });
    }

    [HttpPut("experiments/{experimentId}/result")]
    public async Task<IActionResult> UpdateExperimentResult(int experimentId, [FromBody] UpdateExperimentResultDto dto)
    {
        var experiment = await _context.Experiments.FindAsync(experimentId);
        if (experiment == null) return NotFound();

        experiment.ActualResult = dto.ActualResult;
        experiment.Learnings = dto.Learnings;
        experiment.Status = dto.Status;
        experiment.CompletedAt = System.DateTime.UtcNow;

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-admin";
        await _reputationService.AwardPointsAsync(userId, 50, "Completed an idea experiment");

        await _context.SaveChangesAsync(default);
        return NoContent();
    }
    #endregion
}
