using System;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class IdeaSubscription : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public ParticipantRole Role { get; set; } = ParticipantRole.Audience;
    public string? RoleJustification { get; set; }
    public int ContributionsCount { get; set; }
    public bool ReceiveEmailNotifications { get; set; } = true;
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
}
