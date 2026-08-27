namespace ArrayApp.Application.Ideas.Queries;

public record GetCanvasNodesQuery(int IdeaId, int? SessionId = null) : IRequest<List<IdeaCanvasNodeDto>>;

public class GetCanvasNodesQueryHandler : IRequestHandler<GetCanvasNodesQuery, List<IdeaCanvasNodeDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCanvasNodesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IdeaCanvasNodeDto>> Handle(GetCanvasNodesQuery request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas
            .Include(i => i.KnowledgeGaps)
            .Include(i => i.Actions)
            .Include(i => i.Decisions)
            .Include(i => i.Experiments)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);

        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        var nodes = new List<IdeaCanvasNodeDto>();

        // Synthesize canvas nodes from idea structured dimensions if empty
        nodes.Add(new IdeaCanvasNodeDto
        {
            Id = 1,
            IdeaId = idea.Id,
            SessionId = request.SessionId,
            NodeType = "Problem",
            Content = !string.IsNullOrWhiteSpace(idea.ProblemStatement) ? idea.ProblemStatement : idea.Description,
            PosX = 100,
            PosY = 120,
            ColorHex = "#FDE68A",
            VotesCount = 8,
            AuthorName = "Product Lead"
        });

        nodes.Add(new IdeaCanvasNodeDto
        {
            Id = 2,
            IdeaId = idea.Id,
            SessionId = request.SessionId,
            NodeType = "Hypothesis",
            Content = !string.IsNullOrWhiteSpace(idea.Hypothesis) ? idea.Hypothesis : "Core product hypothesis pending test.",
            PosX = 420,
            PosY = 120,
            ColorHex = "#A7F3D0",
            VotesCount = 14,
            AuthorName = "Researcher"
        });

        nodes.Add(new IdeaCanvasNodeDto
        {
            Id = 3,
            IdeaId = idea.Id,
            SessionId = request.SessionId,
            NodeType = "ValueProp",
            Content = !string.IsNullOrWhiteSpace(idea.ValueProposition) ? idea.ValueProposition : "$250k operational savings.",
            PosX = 740,
            PosY = 120,
            ColorHex = "#BAE6FD",
            VotesCount = 19,
            AuthorName = "Sponsor"
        });

        // Add action item cards
        int idx = 0;
        foreach (var action in idea.Actions.Take(4))
        {
            nodes.Add(new IdeaCanvasNodeDto
            {
                Id = 100 + action.Id,
                IdeaId = idea.Id,
                SessionId = request.SessionId,
                NodeType = "ActionCard",
                Content = $"[Task #{action.Id}]: {action.Title} ({action.Status})",
                PosX = 100 + (idx * 220),
                PosY = 380,
                ColorHex = "#DDD6FE",
                VotesCount = 3,
                AuthorName = action.OwnerUserId ?? "Actioner"
            });
            idx++;
        }

        return nodes;
    }
}
