namespace ArrayApp.Application.Ideas.Queries;

public record GetRoleActionHistoryQuery(int IdeaId) : IRequest<List<RoleActionHistoryDto>>;

public class GetRoleActionHistoryQueryHandler : IRequestHandler<GetRoleActionHistoryQuery, List<RoleActionHistoryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRoleActionHistoryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoleActionHistoryDto>> Handle(GetRoleActionHistoryQuery request, CancellationToken cancellationToken)
    {
        var logs = await _context.ProvenanceLogs
            .AsNoTracking()
            .Where(l => l.IdeaId == request.IdeaId && l.ActionPerformed.StartsWith("RoleAction_"))
            .OrderByDescending(l => l.Timestamp)
            .Take(50)
            .ToListAsync(cancellationToken);

        return logs.Select(l => new RoleActionHistoryDto
        {
            Id = l.Id,
            IdeaId = l.IdeaId,
            ActorName = l.ActorName,
            Role = Enum.TryParse<ParticipantRole>(l.ActorRole, out var role) ? role : ParticipantRole.Audience,
            ActionType = l.ActionPerformed.Replace("RoleAction_", ""),
            Summary = l.Details,
            ExecutedAt = l.Timestamp
        }).ToList();
    }
}
