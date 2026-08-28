namespace ArrayApp.Domain.Events;

public class IdeaOutcomeRealizedEvent : BaseEvent
{
    public IdeaOutcomeRealizedEvent(int ideaId, int outcomeId, string title, double estimatedCostSavings, double revenueGenerated, double roiPercent)
    {
        IdeaId = ideaId;
        OutcomeId = outcomeId;
        Title = title;
        EstimatedCostSavings = estimatedCostSavings;
        RevenueGenerated = revenueGenerated;
        RoiPercent = roiPercent;
    }

    public int IdeaId { get; }
    public int OutcomeId { get; }
    public string Title { get; }
    public double EstimatedCostSavings { get; }
    public double RevenueGenerated { get; }
    public double RoiPercent { get; }
}
