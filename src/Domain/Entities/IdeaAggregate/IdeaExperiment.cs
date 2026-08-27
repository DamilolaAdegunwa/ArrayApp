using System;
using System.Collections.Generic;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class IdeaHypothesis : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public string Statement { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;
    public bool IsValidated { get; set; }
    public List<IdeaExperiment> Experiments { get; set; } = new();
}

public class IdeaExperiment : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public int? HypothesisId { get; set; }
    public IdeaHypothesis? Hypothesis { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Protocol { get; set; } = string.Empty;
    public string RequiredResources { get; set; } = string.Empty;
    public string ExpectedMetric { get; set; } = string.Empty;
    public string? ActualResult { get; set; }
    public string? Learnings { get; set; }

    public ExperimentStatus Status { get; set; } = ExperimentStatus.Proposed;
    public DateTimeOffset? StartedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
}
