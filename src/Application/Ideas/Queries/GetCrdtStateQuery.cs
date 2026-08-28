namespace ArrayApp.Application.Ideas.Queries;

public class CrdtIdeaStateSnapshotDto
{
    public int IdeaId { get; set; }
    public long ServerSequence { get; set; }
    public Dictionary<string, long> VectorClock { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string ProblemStatement { get; set; } = string.Empty;
    public string Hypothesis { get; set; } = string.Empty;
    public string ValueProposition { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public int TotalVotes { get; set; }
    public DateTimeOffset SyncedAt { get; set; } = DateTimeOffset.UtcNow;
}

public record GetCrdtStateQuery(int IdeaId) : IRequest<CrdtIdeaStateSnapshotDto>;

public class GetCrdtStateQueryHandler : IRequestHandler<GetCrdtStateQuery, CrdtIdeaStateSnapshotDto>
{
    private readonly IApplicationDbContext _context;

    public GetCrdtStateQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CrdtIdeaStateSnapshotDto> Handle(GetCrdtStateQuery request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);

        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        var serverSeq = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        return new CrdtIdeaStateSnapshotDto
        {
            IdeaId = idea.Id,
            ServerSequence = serverSeq,
            VectorClock = new Dictionary<string, long>
            {
                { "server", serverSeq }
            },
            Title = idea.Title ?? string.Empty,
            ProblemStatement = !string.IsNullOrWhiteSpace(idea.ProblemStatement) ? idea.ProblemStatement : (idea.Description ?? string.Empty),
            Hypothesis = idea.Hypothesis ?? string.Empty,
            ValueProposition = idea.ValueProposition ?? string.Empty,
            Scope = idea.Scope ?? string.Empty,
            TotalVotes = idea.Upvotes,
            SyncedAt = DateTimeOffset.UtcNow
        };
    }
}
