namespace ArrayApp.Application.Ideas.Queries;

public record GetAIAgentInsightsQuery(int IdeaId, int? SessionId = null) : IRequest<List<AIAgentInsightDto>>;

public class GetAIAgentInsightsQueryHandler : IRequestHandler<GetAIAgentInsightsQuery, List<AIAgentInsightDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAIAgentInsightsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AIAgentInsightDto>> Handle(GetAIAgentInsightsQuery request, CancellationToken cancellationToken)
    {
        var insights = await _context.AIAgentInsights
            .AsNoTracking()
            .Where(i => i.IdeaId == request.IdeaId && (!request.SessionId.HasValue || i.SessionId == request.SessionId.Value))
            .OrderByDescending(i => i.IsPinned)
            .ThenByDescending(i => i.GeneratedAt)
            .Select(i => new AIAgentInsightDto
            {
                Id = i.Id,
                IdeaId = i.IdeaId,
                SessionId = i.SessionId,
                AgentType = i.AgentType,
                AgentName = i.AgentName,
                Title = i.Title,
                Summary = i.Summary,
                FullContent = i.FullContent,
                ConfidenceScore = i.ConfidenceScore,
                IsPinned = i.IsPinned,
                GeneratedAt = i.GeneratedAt
            })
            .ToListAsync(cancellationToken);

        return insights;
    }
}
