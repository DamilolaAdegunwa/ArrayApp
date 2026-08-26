using System;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class KnowledgeGap : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DomainArea { get; set; } = string.Empty; // e.g. "Regulatory", "Technical", "Market"
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public KnowledgeGapStatus Status { get; set; } = KnowledgeGapStatus.Open;

    public string? AssignedToUserId { get; set; }
    public ApplicationUser? AssignedToUser { get; set; }

    public string? ResolutionDetails { get; set; }
    public string? SupportingEvidenceUrl { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
