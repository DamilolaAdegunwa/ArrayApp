using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class InvokeAIAgentCommandTests
{
    [Test]
    public void InvokeAIAgentCommand_Initialization_DevilsAdvocateConfigured()
    {
        var command = new InvokeAIAgentCommand
        {
            IdeaId = 15,
            SessionId = 3,
            AgentType = AIAgentType.Critic,
            CustomPrompt = "Stress test the hardware bill of materials under extreme cold",
            ActorName = "Lead Systems Architect"
        };

        command.IdeaId.Should().Be(15);
        command.SessionId.Should().Be(3);
        command.AgentType.Should().Be(AIAgentType.Critic);
        command.CustomPrompt.Should().NotBeNullOrWhiteSpace();
    }

    [Test]
    public void PinAIAgentInsightCommand_SetsTargetInsightId()
    {
        var command = new PinAIAgentInsightCommand(42);
        command.InsightId.Should().Be(42);
    }
}
