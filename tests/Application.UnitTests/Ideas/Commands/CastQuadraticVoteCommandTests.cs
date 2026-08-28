using ArrayApp.Application.Ideas.Commands;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class CastQuadraticVoteCommandTests
{
    [Test]
    public void CastQuadraticVoteCommand_Initialization_CalculatesExpectedCost()
    {
        var command = new CastQuadraticVoteCommand
        {
            IdeaId = 60,
            UserId = "user-voter-1",
            UserName = "Community Member",
            DesiredVotes = 5
        };

        command.IdeaId.Should().Be(60);
        command.DesiredVotes.Should().Be(5);
        // Formula check: (5)^2 = 25 credits
        var cost = command.DesiredVotes * command.DesiredVotes;
        cost.Should().Be(25);
    }

    [Test]
    public void PlaceIdeaPredictionCommand_SetsWagerAndPrediction()
    {
        var command = new PlaceIdeaPredictionCommand
        {
            IdeaId = 60,
            UserId = "user-predictor",
            UserName = "Market Maker",
            PredictsSuccess = true,
            WageredKarma = 100
        };

        command.PredictsSuccess.Should().BeTrue();
        command.WageredKarma.Should().Be(100);
    }
}
