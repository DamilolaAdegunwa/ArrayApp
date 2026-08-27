using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class ExtractSpeechActionsCommandTests
{
    [Test]
    public void ExtractSpeechActionsCommand_Initialization_ActionStatementParsed()
    {
        var command = new ExtractSpeechActionsCommand
        {
            SessionId = 12,
            IdeaId = 30,
            SpokenTranscript = "I will build the automated API connector by next Tuesday",
            SpeakerName = "Lead Backend Engineer",
            SpeakerUserId = "user-backend",
            SpeakerRole = ParticipantRole.Actioner
        };

        command.SessionId.Should().Be(12);
        command.IdeaId.Should().Be(30);
        command.SpokenTranscript.Should().Contain("will build");
        command.SpeakerRole.Should().Be(ParticipantRole.Actioner);
    }

    [Test]
    public void ExtractedSpeechOutcomesResultDto_SetsDefaults()
    {
        var dto = new ExtractedSpeechOutcomesResultDto
        {
            SessionId = 12,
            IdeaId = 30,
            RealtimeAiSummary = "Extracted 2 actions and 1 decision"
        };

        dto.SessionId.Should().Be(12);
        dto.ExtractedActions.Should().BeEmpty();
        dto.ExtractedDecisions.Should().BeEmpty();
    }
}
