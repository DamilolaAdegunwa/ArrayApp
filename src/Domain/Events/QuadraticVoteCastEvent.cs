namespace ArrayApp.Domain.Events;

public class QuadraticVoteCastEvent : BaseEvent
{
    public QuadraticVoteCastEvent(int ideaId, string userId, int votesCast, int creditCost, int newTotalVotes)
    {
        IdeaId = ideaId;
        UserId = userId;
        VotesCast = votesCast;
        CreditCost = creditCost;
        NewTotalVotes = newTotalVotes;
    }

    public int IdeaId { get; }
    public string UserId { get; }
    public int VotesCast { get; }
    public int CreditCost { get; }
    public int NewTotalVotes { get; }
}
