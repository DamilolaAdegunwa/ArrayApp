namespace ArrayApp.Domain.Events;

public class PlaybookPhaseAdvancedEvent : BaseEvent
{
    public PlaybookPhaseAdvancedEvent(int sessionId, string playbookId, int previousPhase, int newPhase, string phaseName, int durationMinutes)
    {
        SessionId = sessionId;
        PlaybookId = playbookId;
        PreviousPhase = previousPhase;
        NewPhase = newPhase;
        PhaseName = phaseName;
        DurationMinutes = durationMinutes;
    }

    public int SessionId { get; }
    public string PlaybookId { get; }
    public int PreviousPhase { get; }
    public int NewPhase { get; }
    public string PhaseName { get; }
    public int DurationMinutes { get; }
}
