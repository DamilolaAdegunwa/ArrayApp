using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace ArrayApp.Application.IntegrationTests.Ideas;

using static Testing;

public class ProvenanceAndEconomyIntegrationTests : BaseTestFixture
{
    [Test]
    public async Task ShouldCastQuadraticVoteAndIssueW3CCertificate()
    {
        await RunAsDefaultUserAsync();

        // 1. Create Idea
        var ideaId = await SendAsync(new CreateIdeaCommand
        {
            Title = "Decentralized Carbon Credit Oracle",
            Description = "Automated satellite verification of reforestation plots",
            Content = "Specification for verifiable carbon sequestration tracking",
            CategoryId = 1
        });

        // 2. Cast Quadratic Vote (3 votes -> Cost = 3^2 = 9 credits)
        var voteResult = await SendAsync(new CastQuadraticVoteCommand
        {
            IdeaId = ideaId,
            UserId = "user-steward-1",
            UserName = "Steward",
            DesiredVotes = 3
        });

        voteResult.Should().NotBeNull();
        voteResult.IdeaId.Should().Be(ideaId);
        voteResult.VotesCast.Should().Be(3);
        voteResult.CreditCost.Should().Be(9); // 3^2
        voteResult.TotalIdeaVotes.Should().BeGreaterThanOrEqualTo(3);

        // 3. Issue W3C DID Verifiable Realization Certificate
        var cert = await SendAsync(new GenerateProvenanceCertificateCommand(ideaId, "did:arrayapp:org:climate-governance"));

        cert.Should().NotBeNull();
        cert.Id.Should().StartWith("urn:uuid:");
        cert.Issuer.Should().Be("did:arrayapp:org:climate-governance");
        cert.CredentialSubject.IdeaId.Should().Be(ideaId);
        cert.Proof.Type.Should().Be("Ed25519Signature2020");
        cert.Proof.ProofValue.Should().NotBeNullOrWhiteSpace();

        // 4. Verify Cryptographic Provenance Hash Chain
        var provenance = await SendAsync(new VerifyProvenanceChainQuery(ideaId));
        provenance.IsValid.Should().BeTrue();
        provenance.TotalEntriesVerified.Should().BeGreaterThan(0);
        provenance.LatestBlockHash.Should().NotBeNullOrWhiteSpace();
    }
}
