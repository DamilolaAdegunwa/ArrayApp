namespace ArrayApp.Domain.Events;

public class IdeaDimensionsUpdatedEvent : BaseEvent
{
    public IdeaDimensionsUpdatedEvent(Idea idea, double iceScore, double riceScore, int completenessPercentage)
    {
        Idea = idea;
        IceScore = iceScore;
        RiceScore = riceScore;
        CompletenessPercentage = completenessPercentage;
    }

    public Idea Idea { get; }
    public double IceScore { get; }
    public double RiceScore { get; }
    public int CompletenessPercentage { get; }
}
