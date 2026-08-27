using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class SyncActionToConnectorCommandTests
{
    [Test]
    public void SyncActionToConnectorCommand_Initialization_JiraTypeConfigured()
    {
        var command = new SyncActionToConnectorCommand
        {
            ActionId = 104,
            ConnectorType = ConnectorType.Jira,
            ActorName = "Scrum Master"
        };

        command.ActionId.Should().Be(104);
        command.ConnectorType.Should().Be(ConnectorType.Jira);
        command.ActorName.Should().Be("Scrum Master");
    }

    [Test]
    public void ProcessInboundWebhookCommand_ValidatesClosedStatus()
    {
        var command = new ProcessInboundWebhookCommand
        {
            ExternalSystem = "GitHub",
            ExternalReferenceKey = "GH-42",
            Status = "Closed",
            ResolutionComment = "PR merged into main branch"
        };

        command.ExternalSystem.Should().Be("GitHub");
        command.ExternalReferenceKey.Should().Be("GH-42");
        command.Status.Should().Be("Closed");
    }
}
