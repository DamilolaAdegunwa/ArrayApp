namespace ArrayApp.Application.Ideas.Commands;

public record RecordIdeaOutcomeCommand : IRequest<IdeaOutcomeDto>
{
    public int IdeaId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public OutcomeType Type { get; init; } = OutcomeType.BusinessImpact;
    public string? ArtifactUrl { get; init; }
    public double EstimatedCostSavings { get; init; }
    public double RevenueGenerated { get; init; }
    public int ImpactedUsersCount { get; init; }
    public double EstimatedRoiPercent { get; init; }
    public string? RetrospectiveNotes { get; init; }
    public string? KeyLearnings { get; init; }
    public string ActorName { get; init; } = "Executive Sponsor";
}

public class RecordIdeaOutcomeCommandHandler : IRequestHandler<RecordIdeaOutcomeCommand, IdeaOutcomeDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<RecordIdeaOutcomeCommandHandler> _logger;

    public RecordIdeaOutcomeCommandHandler(
        IApplicationDbContext context,
        ILogger<RecordIdeaOutcomeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IdeaOutcomeDto> Handle(RecordIdeaOutcomeCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas
            .Include(i => i.Outcomes)
            .FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);

        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        var outcome = new IdeaOutcome
        {
            IdeaId = idea.Id,
            Title = request.Title,
            Summary = request.Summary,
            Type = request.Type,
            ArtifactUrl = request.ArtifactUrl,
            EstimatedCostSavings = request.EstimatedCostSavings,
            RevenueGenerated = request.RevenueGenerated,
            ImpactedUsersCount = request.ImpactedUsersCount,
            EstimatedRoiPercent = request.EstimatedRoiPercent > 0 ? request.EstimatedRoiPercent : 150.0,
            RetrospectiveNotes = request.RetrospectiveNotes,
            KeyLearnings = request.KeyLearnings,
            RealizedAt = DateTimeOffset.UtcNow,
            CreationTime = DateTimeOffset.UtcNow
        };

        _context.Outcomes.Add(outcome);

        // Advance idea maturity stage to Measured / Implemented
        idea.MaturityStage = IdeaMaturityStage.Measured;
        idea.LastModificationTime = DateTimeOffset.UtcNow;

        // Record Provenance
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = request.ActorName,
            ActorRole = "Executive Sponsor",
            ActionPerformed = "IdeaOutcomeRealized",
            Details = $"Realized Outcome '{outcome.Title}': ${outcome.EstimatedCostSavings:N0} Cost Savings, ${outcome.RevenueGenerated:N0} Revenue, {outcome.ImpactedUsersCount} Users Impacted ({outcome.EstimatedRoiPercent}% ROI)",
            Timestamp = DateTimeOffset.UtcNow
        });

        // Dispatch Domain Event
        idea.AddDomainEvent(new IdeaOutcomeRealizedEvent(
            idea.Id,
            outcome.Id,
            outcome.Title,
            outcome.EstimatedCostSavings,
            outcome.RevenueGenerated,
            outcome.EstimatedRoiPercent
        ));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Realized outcome recorded for Idea {IdeaId}: {Title} (${Savings} savings)", idea.Id, outcome.Title, outcome.EstimatedCostSavings);

        return new IdeaOutcomeDto
        {
            Id = outcome.Id,
            IdeaId = outcome.IdeaId,
            Title = outcome.Title,
            Summary = outcome.Summary,
            Type = outcome.Type,
            ArtifactUrl = outcome.ArtifactUrl,
            EstimatedCostSavings = outcome.EstimatedCostSavings,
            RevenueGenerated = outcome.RevenueGenerated,
            ImpactedUsersCount = outcome.ImpactedUsersCount,
            EstimatedRoiPercent = outcome.EstimatedRoiPercent,
            RetrospectiveNotes = outcome.RetrospectiveNotes,
            KeyLearnings = outcome.KeyLearnings,
            RealizedAt = outcome.RealizedAt
        };
    }
}
