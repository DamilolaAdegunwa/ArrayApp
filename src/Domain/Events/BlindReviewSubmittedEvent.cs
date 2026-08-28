namespace ArrayApp.Domain.Events;

public class BlindReviewSubmittedEvent : BaseEvent
{
    public BlindReviewSubmittedEvent(int ideaId, string reviewerPseudonym, double score, string qualitativeCritique)
    {
        IdeaId = ideaId;
        ReviewerPseudonym = reviewerPseudonym;
        Score = score;
        QualitativeCritique = qualitativeCritique;
    }

    public int IdeaId { get; }
    public string ReviewerPseudonym { get; }
    public double Score { get; }
    public string QualitativeCritique { get; }
}
