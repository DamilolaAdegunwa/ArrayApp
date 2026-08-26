using System;
using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.SessionAggregate;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class AIAgentInsight : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public int? SessionId { get; set; }
    public Session? Session { get; set; }

    public AIAgentType AgentType { get; set; }
    public string AgentName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string FullContent { get; set; } = string.Empty; // Markdown or structured JSON
    public string? PromptUsed { get; set; }

    public double ConfidenceScore { get; set; } = 0.95;
    public bool IsPinned { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
}
