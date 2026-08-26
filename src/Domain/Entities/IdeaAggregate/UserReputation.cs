using System;
using System.Collections.Generic;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class UserReputation : BaseAuditableEntity, IAggregateRoot
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public int TotalPoints { get; set; }
    public int IdeasHelpedCount { get; set; }
    public int ActionsCompletedCount { get; set; }
    public int OutcomesAchievedCount { get; set; }
    public int KnowledgeGapsResolvedCount { get; set; }
    public int SessionsFacilitatedCount { get; set; }

    public string PrimaryReputationTitle { get; set; } = "Idea Explorer"; // e.g. "Idea Catalyst", "Action Leader"
    public List<UserBadge> Badges { get; set; } = new();
}

public class UserBadge : BaseAuditableEntity, IAggregateRoot
{
    public int UserReputationId { get; set; }
    public UserReputation? UserReputation { get; set; }

    public BadgeType BadgeType { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime AwardedAt { get; set; } = DateTime.UtcNow;
}
