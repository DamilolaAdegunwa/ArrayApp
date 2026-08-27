namespace ArrayApp.Domain.Events;

public class IdeasMergedEvent : BaseEvent
{
    public IdeasMergedEvent(Idea sourceIdea, Idea targetIdea, string mergeRationale)
    {
        SourceIdea = sourceIdea;
        TargetIdea = targetIdea;
        MergeRationale = mergeRationale;
    }

    public Idea SourceIdea { get; }
    public Idea TargetIdea { get; }
    public string MergeRationale { get; }
}
