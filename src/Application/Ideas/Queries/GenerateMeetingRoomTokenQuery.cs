namespace ArrayApp.Application.Ideas.Queries;

public class MeetingRoomCredentialsDto
{
    public int SessionId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public string RoomToken { get; set; } = string.Empty;
    public string SfuEndpoint { get; set; } = string.Empty;
    public List<string> IceServers { get; set; } = new();
    public DateTimeOffset ExpiresAt { get; set; } = DateTimeOffset.UtcNow.AddHours(4);
}

public record GenerateMeetingRoomTokenQuery(int SessionId, string UserId, string DisplayName) : IRequest<MeetingRoomCredentialsDto>;

public class GenerateMeetingRoomTokenQueryHandler : IRequestHandler<GenerateMeetingRoomTokenQuery, MeetingRoomCredentialsDto>
{
    private readonly IApplicationDbContext _context;

    public GenerateMeetingRoomTokenQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MeetingRoomCredentialsDto> Handle(GenerateMeetingRoomTokenQuery request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null)
        {
            throw new NotFoundException(nameof(Session), request.SessionId);
        }

        var roomId = $"arrayapp-session-{session.Id}";
        var simulatedToken = $"JWT.WEBRTC.{Guid.NewGuid():N}.{request.UserId}";

        return new MeetingRoomCredentialsDto
        {
            SessionId = session.Id,
            RoomName = roomId,
            RoomToken = simulatedToken,
            SfuEndpoint = "wss://webrtc.arrayapp.io/mesh",
            IceServers = new List<string>
            {
                "stun:stun.l.google.com:19302",
                "stun:stun1.l.google.com:19302"
            },
            ExpiresAt = DateTimeOffset.UtcNow.AddHours(4)
        };
    }
}
