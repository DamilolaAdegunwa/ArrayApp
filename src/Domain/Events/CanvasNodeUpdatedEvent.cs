namespace ArrayApp.Domain.Events;

public class CanvasNodeUpdatedEvent : BaseEvent
{
    public CanvasNodeUpdatedEvent(int ideaId, int? sessionId, int nodeId, string nodeType, double posX, double posY, string content, int votesCount)
    {
        IdeaId = ideaId;
        SessionId = sessionId;
        NodeId = nodeId;
        NodeType = nodeType;
        PosX = posX;
        PosY = posY;
        Content = content;
        VotesCount = votesCount;
    }

    public int IdeaId { get; }
    public int? SessionId { get; }
    public int NodeId { get; }
    public string NodeType { get; }
    public double PosX { get; }
    public double PosY { get; }
    public string Content { get; }
    public int VotesCount { get; }
}
