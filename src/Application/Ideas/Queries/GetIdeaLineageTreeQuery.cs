namespace ArrayApp.Application.Ideas.Queries;

public class IdeaLineageNodeDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string MaturityStage { get; set; } = string.Empty;
    public int? ForkedFromIdeaId { get; set; }
    public int? MergedIntoIdeaId { get; set; }
    public DateTimeOffset Created { get; set; }
    public List<IdeaLineageNodeDto> ChildrenForks { get; set; } = new();
}

public class IdeaLineageTreeDto
{
    public int RootIdeaId { get; set; }
    public IdeaLineageNodeDto RootNode { get; set; } = new();
    public List<IdeaLineageNodeDto> MergedNodes { get; set; } = new();
}

public record GetIdeaLineageTreeQuery(int IdeaId) : IRequest<IdeaLineageTreeDto>;

public class GetIdeaLineageTreeQueryHandler : IRequestHandler<GetIdeaLineageTreeQuery, IdeaLineageTreeDto>
{
    private readonly IApplicationDbContext _context;

    public GetIdeaLineageTreeQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IdeaLineageTreeDto> Handle(GetIdeaLineageTreeQuery request, CancellationToken cancellationToken)
    {
        var currentIdea = await _context.Ideas
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);

        if (currentIdea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        // Trace up to the root ancestor
        int rootId = currentIdea.Id;
        var parentId = currentIdea.ForkedFromIdeaId;
        while (parentId.HasValue && parentId.Value > 0)
        {
            rootId = parentId.Value;
            var parent = await _context.Ideas.AsNoTracking().FirstOrDefaultAsync(i => i.Id == rootId, cancellationToken);
            parentId = parent?.ForkedFromIdeaId;
        }

        // Fetch all ideas in the lineage family
        var family = await _context.Ideas
            .AsNoTracking()
            .Where(i => i.Id == rootId || i.ForkedFromIdeaId == rootId || i.ParentIdeaId == rootId || i.MergedIntoIdeaId == rootId || i.MergedIntoIdeaId == currentIdea.Id)
            .ToListAsync(cancellationToken);

        var rootEntity = family.FirstOrDefault(i => i.Id == rootId) ?? currentIdea;

        var rootNode = new IdeaLineageNodeDto
        {
            Id = rootEntity.Id,
            Title = rootEntity.Title,
            MaturityStage = rootEntity.MaturityStage.ToString(),
            ForkedFromIdeaId = rootEntity.ForkedFromIdeaId,
            MergedIntoIdeaId = rootEntity.MergedIntoIdeaId,
            Created = rootEntity.CreationTime,
            ChildrenForks = family.Where(f => f.ForkedFromIdeaId == rootEntity.Id).Select(f => new IdeaLineageNodeDto
            {
                Id = f.Id,
                Title = f.Title,
                MaturityStage = f.MaturityStage.ToString(),
                ForkedFromIdeaId = f.ForkedFromIdeaId,
                MergedIntoIdeaId = f.MergedIntoIdeaId,
                Created = f.CreationTime
            }).ToList()
        };

        var merged = family.Where(f => f.MergedIntoIdeaId == rootEntity.Id || f.MergedIntoIdeaId == currentIdea.Id).Select(m => new IdeaLineageNodeDto
        {
            Id = m.Id,
            Title = m.Title,
            MaturityStage = m.MaturityStage.ToString(),
            ForkedFromIdeaId = m.ForkedFromIdeaId,
            MergedIntoIdeaId = m.MergedIntoIdeaId,
            Created = m.CreationTime
        }).ToList();

        return new IdeaLineageTreeDto
        {
            RootIdeaId = rootId,
            RootNode = rootNode,
            MergedNodes = merged
        };
    }
}
