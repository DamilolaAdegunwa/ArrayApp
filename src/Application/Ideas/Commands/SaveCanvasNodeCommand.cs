namespace ArrayApp.Application.Ideas.Commands;

public record SaveCanvasNodeCommand : IRequest<IdeaCanvasNodeDto>
{
    public int? Id { get; init; }
    public int IdeaId { get; init; }
    public int? SessionId { get; init; }
    public string NodeType { get; init; } = "Sticky";
    public string Content { get; init; } = string.Empty;
    public double PosX { get; init; }
    public double PosY { get; init; }
    public string ColorHex { get; init; } = "#FEF08A";
    public int VotesCount { get; init; }
    public string? AuthorName { get; init; } = "Innovator";
    public string? ParentNodeId { get; init; }
}

public class SaveCanvasNodeCommandHandler : IRequestHandler<SaveCanvasNodeCommand, IdeaCanvasNodeDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<SaveCanvasNodeCommandHandler> _logger;

    public SaveCanvasNodeCommandHandler(
        IApplicationDbContext context,
        ILogger<SaveCanvasNodeCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IdeaCanvasNodeDto> Handle(SaveCanvasNodeCommand request, CancellationToken cancellationToken)
    {
        var idea = await _context.Ideas.FirstOrDefaultAsync(i => i.Id == request.IdeaId, cancellationToken);
        if (idea == null)
        {
            throw new NotFoundException(nameof(Idea), request.IdeaId);
        }

        var nodeId = request.Id ?? (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() % int.MaxValue);

        // 1. Dispatch Domain Event
        idea.AddDomainEvent(new CanvasNodeUpdatedEvent(
            idea.Id,
            request.SessionId,
            nodeId,
            request.NodeType,
            request.PosX,
            request.PosY,
            request.Content,
            request.VotesCount
        ));

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Canvas Node {NodeId} ({NodeType}) saved on Idea {IdeaId} at ({X}, {Y})", nodeId, request.NodeType, idea.Id, request.PosX, request.PosY);

        return new IdeaCanvasNodeDto
        {
            Id = nodeId,
            IdeaId = request.IdeaId,
            SessionId = request.SessionId,
            NodeType = request.NodeType,
            Content = request.Content,
            PosX = request.PosX,
            PosY = request.PosY,
            ColorHex = request.ColorHex,
            VotesCount = request.VotesCount,
            AuthorName = request.AuthorName,
            ParentNodeId = request.ParentNodeId
        };
    }
}
