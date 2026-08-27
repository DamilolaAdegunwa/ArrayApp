namespace ArrayApp.Application.Ideas.Commands;

public record UpdateIdeaDimensionsCommand : IRequest<IdeaProductDto>
{
    public int IdeaId { get; init; }
    public string ProblemStatement { get; init; } = string.Empty;
    public string Opportunity { get; init; } = string.Empty;
    public string Hypothesis { get; init; } = string.Empty;
    public string TargetAudience { get; init; } = string.Empty;
    public string ValueProposition { get; init; } = string.Empty;
    public string Constraints { get; init; } = string.Empty;
    public string Unknowns { get; init; } = string.Empty;
    public string Evidence { get; init; } = string.Empty;
    public string KeyQuestions { get; init; } = string.Empty;
    public string DesiredOutcome { get; init; } = string.Empty;

    // Prioritization Scoring Inputs
    public double ImpactScore { get; init; } = 8.0;
    public double ConfidenceScore { get; init; } = 8.0;
    public double EaseScore { get; init; } = 7.0;
    public double ReachScore { get; init; } = 500.0;
    public double EffortScore { get; init; } = 3.0;
}

public class UpdateIdeaDimensionsCommandHandler : IRequestHandler<UpdateIdeaDimensionsCommand, IdeaProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateIdeaDimensionsCommandHandler> _logger;

    public UpdateIdeaDimensionsCommandHandler(
        IApplicationDbContext context,
        ILogger<UpdateIdeaDimensionsCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IdeaProductDto> Handle(UpdateIdeaDimensionsCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas
            .Include(i => i.Category)
            .Include(i => i.KnowledgeGaps)
            .Include(i => i.Experiments)
            .Include(i => i.Actions)
            .Include(i => i.Decisions)
            .FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);

        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        // 1. Update 10 Dimensions
        idea.ProblemStatement = request.ProblemStatement;
        idea.Opportunity = request.Opportunity;
        idea.Hypothesis = request.Hypothesis;
        idea.TargetAudience = request.TargetAudience;
        idea.ValueProposition = request.ValueProposition;
        idea.Constraints = request.Constraints;
        idea.Unknowns = request.Unknowns;
        idea.Evidence = request.Evidence;
        idea.DesiredOutcome = request.DesiredOutcome;
        idea.LastModificationTime = DateTimeOffset.UtcNow;

        // 2. Compute Completeness Percentage (out of 10 dimensions)
        int filledCount = 0;
        if (!string.IsNullOrWhiteSpace(request.ProblemStatement)) filledCount++;
        if (!string.IsNullOrWhiteSpace(request.Opportunity)) filledCount++;
        if (!string.IsNullOrWhiteSpace(request.Hypothesis)) filledCount++;
        if (!string.IsNullOrWhiteSpace(request.TargetAudience)) filledCount++;
        if (!string.IsNullOrWhiteSpace(request.ValueProposition)) filledCount++;
        if (!string.IsNullOrWhiteSpace(request.Constraints)) filledCount++;
        if (!string.IsNullOrWhiteSpace(request.Unknowns)) filledCount++;
        if (!string.IsNullOrWhiteSpace(request.Evidence)) filledCount++;
        if (!string.IsNullOrWhiteSpace(request.KeyQuestions)) filledCount++;
        if (!string.IsNullOrWhiteSpace(request.DesiredOutcome)) filledCount++;

        int completenessPercentage = (int)Math.Round((filledCount / 10.0) * 100);

        // 3. Compute Composite Prioritization Formulas
        // ICE = (Impact * Confidence * Ease) / 10
        double safeImpact = Math.Clamp(request.ImpactScore, 1.0, 10.0);
        double safeConfidence = Math.Clamp(request.ConfidenceScore, 1.0, 10.0);
        double safeEase = Math.Clamp(request.EaseScore, 1.0, 10.0);
        double iceScore = Math.Round((safeImpact * safeConfidence * safeEase) / 10.0, 1);

        // RICE = (Reach * Impact * Confidence) / Effort
        double safeEffort = Math.Max(request.EffortScore, 0.5);
        double riceScore = Math.Round((request.ReachScore * safeImpact * (safeConfidence / 10.0)) / safeEffort, 1);

        // 4. Log Immutable Provenance
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = idea.Id,
            ActorName = "System / Facilitator",
            ActorRole = "Product Lead",
            ActionPerformed = "DimensionsUpdated",
            Details = $"Updated 10 dimensions (Completeness: {completenessPercentage}%, ICE Score: {iceScore}, RICE Score: {riceScore})",
            Timestamp = DateTimeOffset.UtcNow
        });

        // 5. Raise Domain Event
        idea.AddDomainEvent(new IdeaDimensionsUpdatedEvent(idea, iceScore, riceScore, completenessPercentage));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Idea {IdeaId} dimensions updated. Completeness: {Completeness}%, ICE: {IceScore}", idea.Id, completenessPercentage, iceScore);

        return new IdeaProductDto
        {
            Id = idea.Id,
            Title = idea.Title,
            Description = idea.Description,
            Content = idea.Content,
            ProblemStatement = idea.ProblemStatement,
            Opportunity = idea.Opportunity,
            Hypothesis = idea.Hypothesis,
            TargetAudience = idea.TargetAudience,
            ValueProposition = idea.ValueProposition,
            Constraints = idea.Constraints,
            Unknowns = idea.Unknowns,
            Evidence = idea.Evidence,
            DesiredOutcome = idea.DesiredOutcome,
            MaturityStage = idea.MaturityStage,
            Visibility = idea.Visibility,
            Rating = idea.Rating,
            Upvotes = idea.Upvotes,
            Downvotes = idea.Downvotes,
            CategoryId = idea.CategoryId,
            CategoryName = idea.Category?.Name,
            Created = idea.CreationTime,
            LastModified = idea.LastModificationTime,
            ImpactScore = safeImpact,
            ConfidenceScore = safeConfidence,
            EaseScore = safeEase,
            KnowledgeGapsCount = idea.KnowledgeGaps.Count,
            ExperimentsCount = idea.Experiments.Count,
            ActionsCount = idea.Actions.Count,
            DecisionsCount = idea.Decisions.Count
        };
    }
}
