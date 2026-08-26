using ArrayApp.Domain.Common;
using ArrayApp.Domain.Common.Interfaces;
using ArrayApp.Domain.Entities.SessionAggregate;

namespace ArrayApp.Domain.Entities.IdeaAggregate;

public class IdeaCanvasNode : BaseAuditableEntity, IAggregateRoot
{
    public int IdeaId { get; set; }
    public Idea? Idea { get; set; }

    public int? SessionId { get; set; }
    public Session? Session { get; set; }

    public string NodeType { get; set; } = "Sticky"; // Sticky, MindMapNode, Risk, Question, Decision, Action
    public string Content { get; set; } = string.Empty;
    public double PosX { get; set; }
    public double PosY { get; set; }
    public string ColorHex { get; set; } = "#FEF08A";
    public int VotesCount { get; set; }

    public string? AuthorName { get; set; }
    public string? ParentNodeId { get; set; }
}
