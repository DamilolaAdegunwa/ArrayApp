using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class ExecuteRoleActionCommandTests
{
    [Test]
    public void ExecuteRoleActionCommand_StudentQuestion_PayloadIsValid()
    {
        var command = new ExecuteRoleActionCommand
        {
            IdeaId = 10,
            UserId = "user-1",
            ActorName = "Alex Rivera",
            Role = ParticipantRole.Student,
            ActionType = "AskInnocentQuestion",
            Payload = "Why can't we use decentralized relays instead of central servers?"
        };

        command.Role.Should().Be(ParticipantRole.Student);
        command.ActionType.Should().Be("AskInnocentQuestion");
        command.Payload.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void ExecuteRoleActionCommand_SponsorPledge_PledgeAmountIsValid()
    {
        var command = new ExecuteRoleActionCommand
        {
            IdeaId = 10,
            UserId = "user-2",
            ActorName = "Dr. Elena Vance",
            Role = ParticipantRole.Sponsor,
            ActionType = "PledgeSponsorship",
            Amount = 50000m,
            Payload = "Committed $50,000 for clinical prototype phase"
        };

        command.Role.Should().Be(ParticipantRole.Sponsor);
        command.Amount.Should().Be(50000m);
    }
}
