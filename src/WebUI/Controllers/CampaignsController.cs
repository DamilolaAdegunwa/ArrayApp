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
public class CampaignsController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IReputationService _reputationService;

    public CampaignsController(IApplicationDbContext context, IReputationService reputationService)
    {
        _context = context;
        _reputationService = reputationService;
    }

    [HttpGet]
    public async Task<ActionResult<List<CampaignDto>>> GetCampaigns()
    {
        var campaigns = await _context.InnovationCampaigns
            .Include(c => c.SubmittedIdeas)
            .OrderByDescending(c => c.StartDate)
            .Select(c => new CampaignDto
            {
                Id = c.Id,
                Title = c.Title,
                ChallengeStatement = c.ChallengeStatement,
                GoalDescription = c.GoalDescription,
                CategoryName = c.CategoryName,
                SponsorOrganization = c.SponsorOrganization,
                RewardPoolAmount = c.RewardPoolAmount,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                IsActive = c.IsActive,
                SubmittedIdeasCount = c.SubmittedIdeas.Count,
                BannerImageUrl = c.BannerImageUrl
            })
            .ToListAsync();

        return Ok(campaigns);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CampaignDetailsDto>> GetCampaignById(int id)
    {
        var campaign = await _context.InnovationCampaigns
            .Include(c => c.SubmittedIdeas)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (campaign == null) return NotFound();

        return Ok(new CampaignDetailsDto
        {
            Id = campaign.Id,
            Title = campaign.Title,
            ChallengeStatement = campaign.ChallengeStatement,
            GoalDescription = campaign.GoalDescription,
            CategoryName = campaign.CategoryName,
            SponsorOrganization = campaign.SponsorOrganization,
            RewardPoolAmount = campaign.RewardPoolAmount,
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            IsActive = campaign.IsActive,
            CustomFormSchemaJson = campaign.CustomFormSchemaJson,
            BannerImageUrl = campaign.BannerImageUrl,
            SubmittedIdeas = campaign.SubmittedIdeas.Select(i => new IdeaSummaryDto
            {
                Id = i.Id,
                Title = i.Title,
                Tagline = i.Tagline,
                MaturityStage = i.MaturityStage.ToString(),
                Rating = i.Rating,
                Upvotes = i.Upvotes
            }).ToList()
        });
    }

    [HttpPost]
    public async Task<ActionResult<CampaignDto>> CreateCampaign([FromBody] CreateCampaignDto dto)
    {
        var campaign = new InnovationCampaign
        {
            Title = dto.Title,
            ChallengeStatement = dto.ChallengeStatement,
            GoalDescription = dto.GoalDescription,
            CategoryName = dto.CategoryName ?? "General",
            SponsorOrganization = dto.SponsorOrganization ?? "Open Community Challenge",
            RewardPoolAmount = dto.RewardPoolAmount,
            StartDate = DateTime.UtcNow,
            EndDate = dto.EndDate ?? DateTime.UtcNow.AddDays(30),
            IsActive = true,
            CustomFormSchemaJson = dto.CustomFormSchemaJson ?? "{}",
            BannerImageUrl = dto.BannerImageUrl ?? ""
        };

        _context.InnovationCampaigns.Add(campaign);
        await _context.SaveChangesAsync(default);

        return CreatedAtAction(nameof(GetCampaignById), new { id = campaign.Id }, new CampaignDto
        {
            Id = campaign.Id,
            Title = campaign.Title,
            ChallengeStatement = campaign.ChallengeStatement,
            GoalDescription = campaign.GoalDescription,
            CategoryName = campaign.CategoryName,
            SponsorOrganization = campaign.SponsorOrganization,
            RewardPoolAmount = campaign.RewardPoolAmount,
            StartDate = campaign.StartDate,
            EndDate = campaign.EndDate,
            IsActive = campaign.IsActive,
            SubmittedIdeasCount = 0
        });
    }

    [HttpPost("{id}/submit-idea")]
    public async Task<ActionResult<IdeaProductDto>> SubmitIdeaToCampaign(int id, [FromBody] SubmitCampaignIdeaDto dto)
    {
        var campaign = await _context.InnovationCampaigns.FindAsync(id);
        if (campaign == null) return NotFound("Campaign not found");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "user-originator";
        var userName = User.Identity?.Name ?? "Idea Originator";

        var idea = new Idea
        {
            CampaignId = id,
            Title = dto.Title,
            Tagline = dto.Tagline,
            Description = dto.Description,
            ProblemStatement = dto.ProblemStatement ?? campaign.ChallengeStatement,
            Opportunity = dto.Opportunity ?? "",
            Hypothesis = dto.Hypothesis ?? "",
            TargetAudience = dto.TargetAudience ?? "",
            ValueProposition = dto.ValueProposition ?? "",
            Constraints = dto.Constraints ?? "",
            Unknowns = dto.Unknowns ?? "",
            Evidence = dto.Evidence ?? "",
            DesiredOutcome = dto.DesiredOutcome ?? campaign.GoalDescription,
            MaturityStage = IdeaMaturityStage.Raw,
            Status = IdeaStatus.Approved,
            Rating = 5.0,
            CategoryId = 1
        };

        _context.Ideas.Add(idea);
        await _context.SaveChangesAsync(default);

        await _reputationService.AwardPointsAsync(userId, 50, $"Submitted a new idea to campaign '{campaign.Title}'");

        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = userName,
            ActorRole = "Creator",
            ActionPerformed = "IdeaSubmittedToCampaign",
            Details = $"Submitted idea '{idea.Title}' under Campaign challenge #{campaign.Id} ({campaign.Title})",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(default);

        return Ok(new IdeaProductDto
        {
            Id = idea.Id,
            Title = idea.Title,
            Tagline = idea.Tagline,
            Description = idea.Description,
            ProblemStatement = idea.ProblemStatement,
            DesiredOutcome = idea.DesiredOutcome,
            MaturityStage = idea.MaturityStage,
            Rating = idea.Rating
        });
    }
}
