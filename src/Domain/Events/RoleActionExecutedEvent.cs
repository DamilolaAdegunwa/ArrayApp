namespace ArrayApp.Domain.Events;

public class RoleActionExecutedEvent : BaseEvent
{
    public RoleActionExecutedEvent(int ideaId, string actorName, ParticipantRole role, string actionType, string summary, int pointsAwarded)
    {
        IdeaId = ideaId;
        ActorName = actorName;
        Role = role;
        ActionType = actionType;
        Summary = summary;
        PointsAwarded = pointsAwarded;
    }

    public int IdeaId { get; }
    public string ActorName { get; }
    public ParticipantRole Role { get; }
    public string ActionType { get; }
    public string Summary { get; }
    public int PointsAwarded { get; }
}
