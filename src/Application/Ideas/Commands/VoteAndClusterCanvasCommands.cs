namespace ArrayApp.Application.Ideas.Commands;

public record VoteCanvasNodeCommand(int IdeaId, int NodeId, int Increment = 1) : IRequest<int>;

public class VoteCanvasNodeCommandHandler : IRequestHandler<VoteCanvasNodeCommand, int>
{
    private readonly IApplicationDbContext _context;

    public VoteCanvasNodeCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(VoteCanvasNodeCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);
        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        idea.Upvotes += request.Increment;
        await _context.SaveChangesAsync(cancellationToken);
        return idea.Upvotes;
    }
}

public record AutoClusterCanvasNodesCommand(int IdeaId, int? SessionId = null) : IRequest<List<IdeaCanvasNodeDto>>;

public class AutoClusterCanvasNodesCommandHandler : IRequestHandler<AutoClusterCanvasNodesCommand, List<IdeaCanvasNodeDto>>
{
    private readonly ISender _mediator;

    public AutoClusterCanvasNodesCommandHandler(ISender mediator)
    {
        _mediator = mediator;
    }

    public async Task<List<IdeaCanvasNodeDto>> Handle(AutoClusterCanvasNodesCommand request, CancellationToken cancellationToken)
    {
        var nodes = await _mediator.Send(new Queries.GetCanvasNodesQuery(request.IdeaId, request.SessionId), cancellationToken);

        // Re-calculate positions into a clean 3-column structured grid
        int col = 0;
        int row = 0;
        foreach (var node in nodes)
        {
            node.PosX = 80 + (col * 300);
            node.PosY = 100 + (row * 180);
            col++;
            if (col >= 3)
            {
                col = 0;
                row++;
            }
        }

        return nodes;
    }
}
