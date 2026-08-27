#pragma warning disable
#pragma info disable
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.WebUI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public LeaderboardController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<LeaderboardEntryDto>>> GetLeaderboard()
    {
        var reputations = await _context.UserReputations
            .Include(ur => ur.Badges)
            .Include(ur => ur.User)
            .OrderByDescending(ur => ur.TotalPoints)
            .ToListAsync();

        int rank = 1;
        var entries = reputations.Select(r => new LeaderboardEntryDto
        {
            Rank = rank++,
            UserId = r.UserId,
            UserName = r.User?.UserName ?? (r.UserId == "user-marcus" ? "Marcus Thorne" : r.UserId == "user-sarah" ? "Sarah Chen" : "Dr. Elena Vance"),
            PrimaryReputationTitle = r.PrimaryReputationTitle,
            TotalPoints = r.TotalPoints,
            IdeasHelpedCount = r.IdeasHelpedCount,
            ActionsCompletedCount = r.ActionsCompletedCount,
            OutcomesAchievedCount = r.OutcomesAchievedCount,
            KnowledgeGapsResolvedCount = r.KnowledgeGapsResolvedCount,
            BadgesCount = r.Badges.Count,
            TopBadges = r.Badges.Select(b => new UserBadgeDto
            {
                Id = b.Id,
                BadgeType = b.BadgeType,
                Title = b.Title,
                Description = b.Description,
                Icon = b.Icon,
                AwardedAt = b.AwardedAt
            }).ToList()
        }).ToList();

        return Ok(entries);
    }

    [HttpGet("nudges")]
    public ActionResult<List<EngagementNudgeDto>> GetNudges([FromQuery] string? userId)
    {
        var nudges = new List<EngagementNudgeDto>
        {
            new EngagementNudgeDto
            {
                Id = 1,
                Title = "🔥 Trending Momentum Alert!",
                Message = "Your idea 'Soil & Crop Health Monitor' is only 3 upvotes away from the #1 spot on the Innovation Campaign Leaderboard!",
                Category = "Trending",
                ActionCta = "View Campaign",
                TargetUrl = "/#campaigns"
            },
            new EngagementNudgeDto
            {
                Id = 2,
                Title = "🎯 Domain Challenge Match (+75 pts Bounty)",
                Message = "A new knowledge gap in 'Spectral Physics & Optical Calibration' was posted. Submit an empirical resolution to claim +75 pts.",
                Category = "KnowledgeGap",
                ActionCta = "Solve Challenge",
                TargetUrl = "/#gaps"
            },
            new EngagementNudgeDto
            {
                Id = 3,
                Title = "🏆 Badge Milestone Ahead",
                Message = "Sarah Chen is 1 task execution away from unlocking the 'Master Action Leader' badge!",
                Category = "Gamification",
                ActionCta = "Complete Ticket",
                TargetUrl = "/#kanban"
            }
        };

        return Ok(nudges);
    }

    [HttpGet("certificates/{ideaId}")]
    public async Task<ActionResult<InnovationCertificateDto>> GetInnovationCertificate(int ideaId)
    {
        var idea = await _context.Ideas
            .Include(i => i.Actions)
            .Include(i => i.Decisions)
            .Include(i => i.Outcomes)
            .FirstOrDefaultAsync(i => i.Id == ideaId);

        var title = idea?.Title ?? "Soil & Crop Health Monitor";

        return Ok(new InnovationCertificateDto
        {
            IdeaId = ideaId,
            CertificateId = $"CERT-{ideaId}-{DateTime.UtcNow.Year}-ARRAY",
            IdeaTitle = title,
            OriginatorName = "Dr. Elena Vance (Professional)",
            SponsorPatronName = "Marcus Thorne (Sponsor)",
            ActionLeadName = "Sarah Chen (Actioner)",
            MaturityAchieved = "Completed & Measured Outcome",
            RealizedSavings = "$45,000+",
            RealizedRoi = "210%",
            VerifiedPillars = "Narrow-band 300nm-900nm Spectrometry, Quantized TinyML 64KB RAM MCU, Swahili Audio Dialect Guidance",
            IssuedDate = DateTime.UtcNow.ToString("MMMM dd, yyyy"),
            CryptographicHash = Guid.NewGuid().ToString("N")
        });
    }
}

public class EngagementNudgeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ActionCta { get; set; } = string.Empty;
    public string TargetUrl { get; set; } = string.Empty;
}

public class InnovationCertificateDto
{
    public int IdeaId { get; set; }
    public string CertificateId { get; set; } = string.Empty;
    public string IdeaTitle { get; set; } = string.Empty;
    public string OriginatorName { get; set; } = string.Empty;
    public string SponsorPatronName { get; set; } = string.Empty;
    public string ActionLeadName { get; set; } = string.Empty;
    public string MaturityAchieved { get; set; } = string.Empty;
    public string RealizedSavings { get; set; } = string.Empty;
    public string RealizedRoi { get; set; } = string.Empty;
    public string VerifiedPillars { get; set; } = string.Empty;
    public string IssuedDate { get; set; } = string.Empty;
    public string CryptographicHash { get; set; } = string.Empty;
}
