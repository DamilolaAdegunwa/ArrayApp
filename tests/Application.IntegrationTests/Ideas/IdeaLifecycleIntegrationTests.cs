using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using ArrayApp.Domain.Entities.CategoryAggregate;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace ArrayApp.Application.IntegrationTests.Ideas;

using static Testing;

public class IdeaLifecycleIntegrationTests : BaseTestFixture
{
    [Test]
    public async Task ShouldCompleteFullInnovationLifecycleFlow()
    {
        await RunAsDefaultUserAsync();

        // 1. Create Base Idea
        var category = await FindAsync<Category>(1);
        var catId = category != null ? category.Id : 1;

        var ideaId = await SendAsync(new CreateIdeaCommand
        {
            Title = "Edge Sensor Crop Optimization",
            Description = "Automated soil microbiome monitoring to minimize aquifer pumping",
            Content = "Detailed architecture for wireless LoRaWAN soil moisture probes",
            CategoryId = catId
        });

        ideaId.Should().BeGreaterThan(0);

        // 2. Update 10-Dimensional Specification
        var updatedDto = await SendAsync(new UpdateIdeaDimensionsCommand
        {
            IdeaId = ideaId,
            ProblemStatement = "Aquifer depletion exceeding 40% sustainable threshold.",
            Opportunity = "IoT micro-dosing reduces extraction by 35%.",
            Hypothesis = "Automated moisture triggers increase crop resilience.",
            TargetAudience = "Commercial grain growers.",
            ValueProposition = "$300k seasonal savings per sector.",
            Constraints = "Battery longevity > 5 years.",
            Unknowns = "Nitrogen sensor corrosion rate.",
            Evidence = "Field trials in sector 4.",
            KeyQuestions = "What is the wireless packet loss through wet soil?",
            DesiredOutcome = "Full automated cloud irrigation valve controller.",
            ImpactScore = 8.5,
            ConfidenceScore = 9.0,
            EaseScore = 7.0,
            ReachScore = 15000.0,
            EffortScore = 3.0
        });

        updatedDto.MaturityStage.Should().Be(IdeaMaturityStage.Structured);
        updatedDto.Rating.Should().BeGreaterThanOrEqualTo(0);

        // 3. Fork Idea for Specialized Arctic Variant
        var forkedDto = await SendAsync(new ForkIdeaCommand
        {
            IdeaId = ideaId,
            NewTitle = "Edge Sensor Arctic Variant",
            ForkRationale = "Specialized thermal insulation for sub-zero tundra greenhouses",
            ActorName = "Thermal Engineer"
        });

        forkedDto.Id.Should().BeGreaterThan(0);
        forkedDto.Id.Should().NotBe(ideaId);

        var forkedIdea = await FindAsync<Idea>(forkedDto.Id);
        forkedIdea.Should().NotBeNull();
        forkedIdea!.ForkedFromIdeaId.Should().Be(ideaId);

        // 4. Verify Cryptographic Provenance Chain
        var provenance = await SendAsync(new VerifyProvenanceChainQuery(ideaId));
        provenance.IsValid.Should().BeTrue();
        provenance.TotalEntriesVerified.Should().BeGreaterThan(0);
        provenance.LatestBlockHash.Should().NotBeNullOrWhiteSpace();
    }
}
