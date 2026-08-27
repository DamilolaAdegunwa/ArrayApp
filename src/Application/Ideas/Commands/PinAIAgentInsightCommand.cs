namespace ArrayApp.Application.Ideas.Commands;

public record PinAIAgentInsightCommand(int InsightId) : IRequest<bool>;

public class PinAIAgentInsightCommandHandler : IRequestHandler<PinAIAgentInsightCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public PinAIAgentInsightCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(PinAIAgentInsightCommand request, CancellationToken cancellationToken)
    {
        var insight = await _context.AIAgentInsights.FindAsync(new object[] { request.InsightId }, cancellationToken);
        if (insight == null)
        {
            throw new NotFoundException(nameof(AIAgentInsight), request.InsightId);
        }

        insight.IsPinned = !insight.IsPinned;
        await _context.SaveChangesAsync(cancellationToken);
        return insight.IsPinned;
    }
}
