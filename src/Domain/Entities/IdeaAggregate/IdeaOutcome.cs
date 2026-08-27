using System;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class IdeaOutcome : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public OutcomeType Type { get; set; } = OutcomeType.Prototype;

    public string? ArtifactUrl { get; set; }
    public double EstimatedCostSavings { get; set; }
    public double RevenueGenerated { get; set; }
    public int ImpactedUsersCount { get; set; }
    public double EstimatedRoiPercent { get; set; }

    public string? RetrospectiveNotes { get; set; }
    public string? KeyLearnings { get; set; }
    public DateTimeOffset RealizedAt { get; set; } = DateTimeOffset.UtcNow;
}
