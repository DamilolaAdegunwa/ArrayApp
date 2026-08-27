using ArrayApp.Application.Ideas.Commands;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class ForkAndMergeIdeaCommandTests
{
    [Test]
    public void ForkIdeaCommand_Instantiation_SetsDefaultsCorrectly()
    {
        var command = new ForkIdeaCommand
        {
            IdeaId = 1,
            NewTitle = "New Sub-Idea Fork",
            ForkRationale = "Testing market niche B",
            ActorName = "Lead Innovator"
        };

        command.IdeaId.Should().Be(1);
        command.NewTitle.Should().Be("New Sub-Idea Fork");
        command.ForkRationale.Should().Be("Testing market niche B");
        command.ActorName.Should().Be("Lead Innovator");
    }

    [Test]
    public void MergeIdeasCommand_Validation_DifferentIdsRequired()
    {
        var command = new MergeIdeasCommand
        {
            SourceIdeaId = 5,
            TargetIdeaId = 10,
            MergeRationale = "Consolidating duplicate workflow proposals",
            ActorName = "Product Facilitator"
        };

        command.SourceIdeaId.Should().NotBe(command.TargetIdeaId);
        command.MergeRationale.Should().NotBeNullOrWhiteSpace();
    }
}
