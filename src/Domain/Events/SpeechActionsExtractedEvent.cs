namespace ArrayApp.Domain.Events;

public class SpeechActionsExtractedEvent : BaseEvent
{
    public SpeechActionsExtractedEvent(int sessionId, int ideaId, int actionsCount, int decisionsCount, string speakerSummary)
    {
        SessionId = sessionId;
        IdeaId = ideaId;
        ActionsCount = actionsCount;
        DecisionsCount = decisionsCount;
        SpeakerSummary = speakerSummary;
    }

    public int SessionId { get; }
    public int IdeaId { get; }
    public int ActionsCount { get; }
    public int DecisionsCount { get; }
    public string SpeakerSummary { get; }
}
