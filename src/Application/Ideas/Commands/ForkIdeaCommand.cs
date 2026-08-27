namespace ArrayApp.Application.Ideas.Commands;

public record ForkIdeaCommand : IRequest<IdeaProductDto>
{
    public int IdeaId { get; init; }
    public string NewTitle { get; init; } = string.Empty;
    public string? ForkRationale { get; init; }
    public string ActorName { get; init; } = "Innovator";
}

public class ForkIdeaCommandHandler : IRequestHandler<ForkIdeaCommand, IdeaProductDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<ForkIdeaCommandHandler> _logger;

    public ForkIdeaCommandHandler(
        IApplicationDbContext context,
        ILogger<ForkIdeaCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IdeaProductDto> Handle(ForkIdeaCommand request, CancellationToken cancellationToken)
    {
        var originalIdea = await _context.Ideas
            .Include(i => i.Category)
            .Include(i => i.KnowledgeGaps)
            .Include(i => i.Experiments)
            .FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);

        if (originalIdea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        var forkedTitle = !string.IsNullOrWhiteSpace(request.NewTitle)
            ? request.NewTitle
            : $"{originalIdea.Title} (Fork)";

        // 1. Create Forked Child Idea with cloned 10 dimensions
        var forkedIdea = new Idea
        {
            Title = forkedTitle,
            Tagline = originalIdea.Tagline,
            Description = originalIdea.Description,
            Content = originalIdea.Content,
            ProblemStatement = originalIdea.ProblemStatement,
            Opportunity = originalIdea.Opportunity,
            Hypothesis = originalIdea.Hypothesis,
            TargetAudience = originalIdea.TargetAudience,
            ValueProposition = originalIdea.ValueProposition,
            Constraints = originalIdea.Constraints,
            Unknowns = originalIdea.Unknowns,
            Evidence = originalIdea.Evidence,
            DesiredOutcome = originalIdea.DesiredOutcome,
            MaturityStage = IdeaMaturityStage.Exploring,
            Visibility = originalIdea.Visibility,
            CategoryId = originalIdea.CategoryId,
            ForkedFromIdeaId = originalIdea.Id,
            ParentIdeaId = originalIdea.Id,
            Rating = 5.0,
            Upvotes = 1,
            CreationTime = DateTimeOffset.UtcNow,
            LastModificationTime = DateTimeOffset.UtcNow
        };

        _context.Ideas.Add(forkedIdea);
        await _context.SaveChangesAsync(cancellationToken);

        // 2. Clone Open Knowledge Gaps to Fork
        if (originalIdea.KnowledgeGaps != null && originalIdea.KnowledgeGaps.Any())
        {
            foreach (var gap in originalIdea.KnowledgeGaps.Where(g => g.Status != KnowledgeGapStatus.Resolved))
            {
                _context.KnowledgeGaps.Add(new KnowledgeGap
                {
                    IdeaId = forkedIdea.Id,
                    Title = gap.Title,
                    Description = gap.Description,
                    DomainArea = gap.DomainArea,
                    Priority = gap.Priority,
                    Status = KnowledgeGapStatus.Open,
                    CreationTime = DateTimeOffset.UtcNow
                });
            }
        }

        // 3. Log Provenance for both Original and Forked Ideas
        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = originalIdea.Id,
            ActorName = request.ActorName,
            ActorRole = "Contributor",
            ActionPerformed = "IdeaForkedOut",
            Details = $"Idea forked into Child Idea #{forkedIdea.Id} ('{forkedIdea.Title}'). Rationale: {request.ForkRationale ?? "Exploratory spin-off"}",
            Timestamp = DateTimeOffset.UtcNow
        });

        _context.ProvenanceLogs.Add(new ProvenanceLog
        {
            IdeaId = forkedIdea.Id,
            ActorName = request.ActorName,
            ActorRole = "Author",
            ActionPerformed = "IdeaForkedIn",
            Details = $"Idea created as fork of Parent Idea #{originalIdea.Id} ('{originalIdea.Title}'). Rationale: {request.ForkRationale ?? "Exploratory spin-off"}",
            Timestamp = DateTimeOffset.UtcNow
        });

        // 4. Dispatch Domain Event
        forkedIdea.AddDomainEvent(new IdeaForkedEvent(originalIdea, forkedIdea));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Idea {OriginalId} forked into new Idea {ForkedId}", originalIdea.Id, forkedIdea.Id);

        return new IdeaProductDto
        {
            Id = forkedIdea.Id,
            Title = forkedIdea.Title,
            Tagline = forkedIdea.Tagline,
            Description = forkedIdea.Description,
            ProblemStatement = forkedIdea.ProblemStatement,
            Opportunity = forkedIdea.Opportunity,
            Hypothesis = forkedIdea.Hypothesis,
            TargetAudience = forkedIdea.TargetAudience,
            ValueProposition = forkedIdea.ValueProposition,
            Constraints = forkedIdea.Constraints,
            Unknowns = forkedIdea.Unknowns,
            Evidence = forkedIdea.Evidence,
            DesiredOutcome = forkedIdea.DesiredOutcome,
            MaturityStage = forkedIdea.MaturityStage,
            Visibility = forkedIdea.Visibility,
            Rating = forkedIdea.Rating,
            Upvotes = forkedIdea.Upvotes,
            CategoryId = forkedIdea.CategoryId,
            CategoryName = originalIdea.Category?.Name,
            ForkedFromIdeaId = originalIdea.Id,
            ParentIdeaId = originalIdea.Id,
            Created = forkedIdea.CreationTime,
            LastModified = forkedIdea.LastModificationTime
        };
    }
}
