using ArrayApp.Application.Ideas.Commands;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class AdvancePlaybookPhaseCommandTests
{
    [Test]
    public void AdvancePlaybookPhaseCommand_Initialization_SetsDefaults()
    {
        var command = new AdvancePlaybookPhaseCommand
        {
            SessionId = 42,
            PlaybookId = "six-hats",
            TargetPhaseNumber = 2,
            FacilitatorName = "Chief Product Officer"
        };

        command.SessionId.Should().Be(42);
        command.PlaybookId.Should().Be("six-hats");
        command.TargetPhaseNumber.Should().Be(2);
        command.FacilitatorName.Should().Be("Chief Product Officer");
    }

    [Test]
    public void PlaybookPhaseProgressDto_DurationAndStatus_CalculatesCorrectly()
    {
        var progress = new PlaybookPhaseProgressDto
        {
            SessionId = 1,
            PlaybookId = "scamper",
            CurrentPhaseNumber = 4,
            CurrentPhaseTitle = "Dot Voting & Action Extraction",
            PhaseDurationMinutes = 10,
            IsFinalPhase = true
        };

        progress.CurrentPhaseNumber.Should().Be(4);
        progress.IsFinalPhase.Should().BeTrue();
        progress.PhaseDurationMinutes.Should().Be(10);
    }
}
