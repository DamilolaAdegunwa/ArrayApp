using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.WebAPI.Controllers;

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
}
