using ArrayApp.Application.Ideas.Commands;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class AnonymizedReviewCommandTests
{
    [Test]
    public void AnonymizedReviewCompletedCommand_Initialization_PseudonymAssigned()
    {
        var command = new AnonymizedReviewCompletedCommand
        {
            IdeaId = 88,
            ReviewerPseudonym = "Reviewer-Gamma-77",
            Score = 9.2,
            QualitativeCritique = "Outstanding technical feasibility and strong unit economics.",
            Recommendation = "Fast-Track Approval"
        };

        command.IdeaId.Should().Be(88);
        command.ReviewerPseudonym.Should().Be("Reviewer-Gamma-77");
        command.Score.Should().Be(9.2);
        command.Recommendation.Should().Be("Fast-Track Approval");
    }
}
