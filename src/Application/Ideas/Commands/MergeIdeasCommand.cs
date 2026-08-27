namespace ArrayApp.Application.Ideas.Commands;

public record MergeIdeasCommand : IRequest<IdeaProductDto>
{
    public int SourceIdeaId { get; init; }
    public int TargetIdeaId { get; init; }
    public string MergeRationale { get; init; } = string.Empty;
    public string ActorName { get; init; } = "Facilitator";
}

public class MergeIdeasCommandHandler : IRequestHandler<MergeIdeasCommand, IdeaProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<MergeIdeasCommandHandler> _logger;

    public MergeIdeasCommandHandler(
        IApplicationDbContext context,
        ILogger<MergeIdeasCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IdeaProductDto> Handle(MergeIdeasCommand request, CancellationToken cancellationToken)
    {
        if (request.SourceIdeaId == request.TargetIdeaId)
        {
            throw new ArrayApp.Application.Common.Exceptions.ValidationException(new List<FluentValidation.Results.ValidationFailure>
            {
                new FluentValidation.Results.ValidationFailure("SourceIdeaId", "Cannot merge an idea into itself.")
            });
        }

        var sourceIdea = await _context.Ideas
            .Include(i => i.KnowledgeGaps)
            .Include(i => i.Experiments)
            .Include(i => i.Actions)
            .Include(i => i.Decisions)
            .FirstOrDefaultAsync(i => i.Id == request.SourceIdeaId, cancellationToken);

        if (sourceIdea == null)
        {
            throw new NotFoundException(nameof(Idea), request.SourceIdeaId);
        }

        var targetIdea = await _context.Ideas
            .Include(i => i.Category)
            .Include(i => i.KnowledgeGaps)
            .Include(i => i.Experiments)
            .Include(i => i.Actions)
            .Include(i => i.Decisions)
            .FirstOrDefaultAsync(i => i.Id == request.TargetIdeaId, cancellationToken);

        if (targetIdea == null)
        {
            throw new NotFoundException(nameof(Idea), request.TargetIdeaId);
        }

        // 1. Consolidate Evidence and Supporting details into Target Idea
        if (!string.IsNullOrWhiteSpace(sourceIdea.Evidence) && !targetIdea.Evidence.Contains(sourceIdea.Evidence))
        {
            targetIdea.Evidence = string.IsNullOrWhiteSpace(targetIdea.Evidence)
                ? sourceIdea.Evidence
                : $"{targetIdea.Evidence}\n[Merged from #{sourceIdea.Id}]: {sourceIdea.Evidence}";
        }

        // 2. Re-point source child actions, knowledge gaps, and experiments to target idea
        foreach (var action in sourceIdea.Actions)
        {
            action.IdeaId = targetIdea.Id;
        }

        foreach (var gap in sourceIdea.KnowledgeGaps)
        {
            gap.IdeaId = targetIdea.Id;
        }

        foreach (var exp in sourceIdea.Experiments)
        {
            exp.IdeaId = targetIdea.Id;
        }

        // 3. Mark Source Idea as Merged
        sourceIdea.MergedIntoIdeaId = targetIdea.Id;
        sourceIdea.MaturityStage = IdeaMaturityStage.Evolving;
        sourceIdea.LastModificationTime = DateTimeOffset.UtcNow;
        targetIdea.LastModificationTime = DateTimeOffset.UtcNow;

        // 4. Log Immutable Provenance on both
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = sourceIdea.Id,
            ActorName = request.ActorName,
            ActorRole = "Facilitator",
            ActionPerformed = "IdeaMergedInto",
            Details = $"Merged into Target Idea #{targetIdea.Id} ('{targetIdea.Title}'). Rationale: {request.MergeRationale}",
            Timestamp = DateTimeOffset.UtcNow
        });

        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = targetIdea.Id,
            ActorName = request.ActorName,
            ActorRole = "Facilitator",
            ActionPerformed = "IdeaMergedFrom",
            Details = $"Absorbed Source Idea #{sourceIdea.Id} ('{sourceIdea.Title}'). Rationale: {request.MergeRationale}",
            Timestamp = DateTimeOffset.UtcNow
        });

        // 5. Dispatch Domain Event
        targetIdea.AddDomainEvent(new IdeasMergedEvent(sourceIdea, targetIdea, request.MergeRationale));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Source Idea {SourceId} successfully merged into Target Idea {TargetId}", sourceIdea.Id, targetIdea.Id);

        return new IdeaProductDto
        {
            Id = targetIdea.Id,
            Title = targetIdea.Title,
            Tagline = targetIdea.Tagline,
            Description = targetIdea.Description,
            ProblemStatement = targetIdea.ProblemStatement,
            Opportunity = targetIdea.Opportunity,
            Hypothesis = targetIdea.Hypothesis,
            TargetAudience = targetIdea.TargetAudience,
            ValueProposition = targetIdea.ValueProposition,
            Constraints = targetIdea.Constraints,
            Unknowns = targetIdea.Unknowns,
            Evidence = targetIdea.Evidence,
            DesiredOutcome = targetIdea.DesiredOutcome,
            MaturityStage = targetIdea.MaturityStage,
            Visibility = targetIdea.Visibility,
            Rating = targetIdea.Rating,
            Upvotes = targetIdea.Upvotes,
            CategoryId = targetIdea.CategoryId,
            CategoryName = targetIdea.Category?.Name,
            MergedIntoIdeaId = null,
            Created = targetIdea.CreationTime,
            LastModified = targetIdea.LastModificationTime,
            KnowledgeGapsCount = targetIdea.KnowledgeGaps.Count,
            ExperimentsCount = targetIdea.Experiments.Count,
            ActionsCount = targetIdea.Actions.Count,
            DecisionsCount = targetIdea.Decisions.Count
        };
    }
}
