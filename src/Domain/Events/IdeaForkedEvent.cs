namespace ArrayApp.Domain.Events;

public class IdeaForkedEvent : BaseEvent
{
    public IdeaForkedEvent(Idea parentIdea, Idea forkedIdea)
    {
        ParentIdea = parentIdea;
        ForkedIdea = forkedIdea;
    }

    public Idea ParentIdea { get; }
    public Idea ForkedIdea { get; }
}
