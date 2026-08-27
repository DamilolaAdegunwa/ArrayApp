namespace ArrayApp.Application.Ideas.Queries;

public record GetIdeaProductDimensionsQuery(int IdeaId) : IRequest<IdeaProductDto>;

public class GetIdeaProductDimensionsQueryHandler : IRequestHandler<GetIdeaProductDimensionsQuery, IdeaProductDto>
{
    private readonly IApplicationDbContext _context;

    public GetIdeaProductDimensionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IdeaProductDto> Handle(GetIdeaProductDimensionsQuery request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas
            .Include(i => i.Category)
            .Include(i => i.KnowledgeGaps)
            .Include(i => i.Experiments)
            .Include(i => i.Actions)
            .Include(i => i.Decisions)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);

        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

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
            ImpactScore = 8.5,
            ConfidenceScore = 8.0,
            EaseScore = 7.0,
            KnowledgeGapsCount = idea.KnowledgeGaps.Count,
            ExperimentsCount = idea.Experiments.Count,
            ActionsCount = idea.Actions.Count,
            DecisionsCount = idea.Decisions.Count
        };
    }
}
