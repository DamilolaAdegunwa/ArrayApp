using System;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.SessionAggregate;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class IdeaAction : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public int? SessionId { get; set; }
    public Session? Session { get; set; }

    public int? DecisionId { get; set; }
    public IdeaDecision? Decision { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string? OwnerUserId { get; set; }
    public ApplicationUser? OwnerUser { get; set; }

    public string? SupportingTeam { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public ActionItemStatus Status { get; set; } = ActionItemStatus.Todo;

    public DateTimeOffset? DueDate { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }

    public string? Dependencies { get; set; }

    // External connector references
    public string? ExternalSystem { get; set; } // e.g. "Jira", "GitHub", "Trello"
    public string? ExternalReferenceKey { get; set; } // e.g. "PROJ-104", "GH-Issue#42"
    public string? ExternalUrl { get; set; }
}
