namespace ArrayApp.Domain.Events;

public class AIAgentInsightGeneratedEvent : BaseEvent
{
    public AIAgentInsightGeneratedEvent(int ideaId, int insightId, AIAgentType agentType, string agentName, string title, double confidenceScore)
    {
        IdeaId = ideaId;
        InsightId = insightId;
        AgentType = agentType;
        AgentName = agentName;
        Title = title;
        ConfidenceScore = confidenceScore;
    }

    public int IdeaId { get; }
    public int InsightId { get; }
    public AIAgentType AgentType { get; }
    public string AgentName { get; }
    public string Title { get; }
    public double ConfidenceScore { get; }
}
