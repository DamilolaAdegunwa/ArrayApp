#pragma warning disable
#pragma info disable
using System;
using System.Collections.Generic;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.SessionAggregate;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class IdeaDecision : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public int? SessionId { get; set; }
    public Session? Session { get; set; }

    public string Summary { get; set; } = string.Empty;
    public string Rationale { get; set; } = string.Empty;
    public string Context { get; set; } = string.Empty;
    public string? DecidedByUserId { get; set; }
    public ApplicationUser? DecidedByUser { get; set; }

    public DateTimeOffset DecidedAt { get; set; } = DateTimeOffset.UtcNow;
    public List<IdeaAction> ResultingActions { get; set; } = new();
}
