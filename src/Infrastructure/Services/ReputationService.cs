using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ArrayApp.Application.Common.Interfaces;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ArrayApp.Infrastructure.Services;

public class ReputationService : IReputationService
{
    private readonly IApplicationDbContext _context;

    public ReputationService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AwardPointsAsync(string userId, int points, string reason, CancellationToken cancellationToken = default)
    {
        var reputation = await _context.UserReputations
            .Include(ur => ur.Badges)
            .FirstOrDefaultAsync(ur => ur.UserId == userId, cancellationToken);

        if (reputation == null)
        {
            reputation = new UserReputation
            {
                UserId = userId,
                TotalPoints = points,
                PrimaryReputationTitle = "Idea Explorer"
            };
            _context.UserReputations.Add(reputation);
        }
        else
        {
            reputation.TotalPoints += points;
        }

        await CheckAndAwardBadgesAsync(userId, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task CheckAndAwardBadgesAsync(string userId, CancellationToken cancellationToken = default)
    {
        var reputation = await _context.UserReputations
            .Include(ur => ur.Badges)
            .FirstOrDefaultAsync(ur => ur.UserId == userId, cancellationToken);

        if (reputation == null) return;

        // Badge 1: Idea Catalyst
        if (reputation.IdeasHelpedCount >= 3 && !reputation.Badges.Any(b => b.BadgeType == BadgeType.IdeaCatalyst))
        {
            reputation.Badges.Add(new UserBadge
            {
                BadgeType = BadgeType.IdeaCatalyst,
                Title = "Idea Catalyst",
                Description = "Helped accelerate multiple ideas through active contribution.",
                Icon = "⚡",
                AwardedAt = DateTime.UtcNow
            });
            reputation.PrimaryReputationTitle = "Idea Catalyst";
        }

        // Badge 2: Action Leader
        if (reputation.ActionsCompletedCount >= 3 && !reputation.Badges.Any(b => b.BadgeType == BadgeType.ActionLeader))
        {
            reputation.Badges.Add(new UserBadge
            {
                BadgeType = BadgeType.ActionLeader,
                Title = "Action Leader",
                Description = "Converted strategic session decisions into executed deliverables.",
                Icon = "🛠️",
                AwardedAt = DateTime.UtcNow
            });
            reputation.PrimaryReputationTitle = "Action Leader";
        }

        // Badge 3: Knowledge Contributor
        if (reputation.KnowledgeGapsResolvedCount >= 2 && !reputation.Badges.Any(b => b.BadgeType == BadgeType.KnowledgeContributor))
        {
            reputation.Badges.Add(new UserBadge
            {
                BadgeType = BadgeType.KnowledgeContributor,
                Title = "Knowledge Contributor",
                Description = "Filled critical knowledge gaps and provided essential evidence.",
                Icon = "📚",
                AwardedAt = DateTime.UtcNow
            });
        }

        // Badge 4: Idea Builder
        if (reputation.OutcomesAchievedCount >= 1 && !reputation.Badges.Any(b => b.BadgeType == BadgeType.IdeaBuilder))
        {
            reputation.Badges.Add(new UserBadge
            {
                BadgeType = BadgeType.IdeaBuilder,
                Title = "Idea Builder",
                Description = "Shepherded an idea all the way to a validated tangible outcome.",
                Icon = "🏆",
                AwardedAt = DateTime.UtcNow
            });
            reputation.PrimaryReputationTitle = "Master Idea Builder";
        }
    }

    public async Task RecordIdeaContributionAsync(string userId, int ideaId, ParticipantRole role, CancellationToken cancellationToken = default)
    {
        var reputation = await _context.UserReputations
            .Include(ur => ur.Badges)
            .FirstOrDefaultAsync(ur => ur.UserId == userId, cancellationToken);

        if (reputation == null)
        {
            reputation = new UserReputation
            {
                UserId = userId,
                TotalPoints = 20,
                IdeasHelpedCount = 1,
                PrimaryReputationTitle = role.ToString()
            };
            _context.UserReputations.Add(reputation);
        }
        else
        {
            reputation.TotalPoints += 20;
            reputation.IdeasHelpedCount++;
        }

        await CheckAndAwardBadgesAsync(userId, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordActionCompletionAsync(string userId, int actionId, CancellationToken cancellationToken = default)
    {
        var reputation = await _context.UserReputations
            .Include(ur => ur.Badges)
            .FirstOrDefaultAsync(ur => ur.UserId == userId, cancellationToken);

        if (reputation == null)
        {
            reputation = new UserReputation
            {
                UserId = userId,
                TotalPoints = 50,
                ActionsCompletedCount = 1,
                PrimaryReputationTitle = "Action Lead"
            };
            _context.UserReputations.Add(reputation);
        }
        else
        {
            reputation.TotalPoints += 50;
            reputation.ActionsCompletedCount++;
        }

        await CheckAndAwardBadgesAsync(userId, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordOutcomeAchievementAsync(string userId, int outcomeId, CancellationToken cancellationToken = default)
    {
        var reputation = await _context.UserReputations
            .Include(ur => ur.Badges)
            .FirstOrDefaultAsync(ur => ur.UserId == userId, cancellationToken);

        if (reputation == null)
        {
            reputation = new UserReputation
            {
                UserId = userId,
                TotalPoints = 150,
                OutcomesAchievedCount = 1,
                PrimaryReputationTitle = "Outcome Champion"
            };
            _context.UserReputations.Add(reputation);
        }
        else
        {
            reputation.TotalPoints += 150;
            reputation.OutcomesAchievedCount++;
        }

        await CheckAndAwardBadgesAsync(userId, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
