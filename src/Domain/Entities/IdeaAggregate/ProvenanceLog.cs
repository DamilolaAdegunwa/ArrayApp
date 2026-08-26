using System;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class ProvenanceLog : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public string ActorName { get; set; } = string.Empty;
    public string? ActorRole { get; set; }
    public string ActionPerformed { get; set; } = string.Empty; // "IdeaCreated", "MaturityAdvanced", "DecisionMade", "ActionExtracted", "AgentAnalysis"
    public string Details { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
