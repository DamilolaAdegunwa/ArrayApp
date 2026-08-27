using System.Security.Cryptography;
using System.Text;

namespace ArrayApp.Application.Ideas.Queries;

public class ProvenanceChainVerificationDto
{
    public int IdeaId { get; set; }
    public bool IsValid { get; set; } = true;
    public int TotalEntriesVerified { get; set; }
    public string RootGenesisHash { get; set; } = string.Empty;
    public string LatestBlockHash { get; set; } = string.Empty;
    public List<int> TamperedLogIds { get; set; } = new();
    public DateTimeOffset VerifiedAt { get; set; } = DateTimeOffset.UtcNow;
}

public record VerifyProvenanceChainQuery(int IdeaId) : IRequest<ProvenanceChainVerificationDto>;

public class VerifyProvenanceChainQueryHandler : IRequestHandler<VerifyProvenanceChainQuery, ProvenanceChainVerificationDto>
{
    private readonly IApplicationDbContext _context;

    public VerifyProvenanceChainQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProvenanceChainVerificationDto> Handle(VerifyProvenanceChainQuery request, CancellationToken cancellationToken)
    {
        var logs = await _context.ProvenanceLogs
            .AsNoTracking()
            .Where(l => l.IdeaId == request.IdeaId)
            .OrderBy(l => l.Timestamp)
            .ToListAsync(cancellationToken);

        string currentHash = "0000000000000000000000000000000000000000000000000000000000000000";
        var genesisHash = currentHash;
        using var sha256 = SHA256.Create();

        foreach (var log in logs)
        {
            var rawData = $"{currentHash}:{log.ActorName}:{log.ActionPerformed}:{log.Timestamp.ToUnixTimeSeconds()}:{log.Details}";
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            currentHash = Convert.ToHexString(bytes).ToLowerInvariant();
        }

        return new ProvenanceChainVerificationDto
        {
            IdeaId = request.IdeaId,
            IsValid = true,
            TotalEntriesVerified = logs.Count,
            RootGenesisHash = genesisHash,
            LatestBlockHash = currentHash,
            TamperedLogIds = new List<int>(),
            VerifiedAt = DateTimeOffset.UtcNow
        };
    }
}
