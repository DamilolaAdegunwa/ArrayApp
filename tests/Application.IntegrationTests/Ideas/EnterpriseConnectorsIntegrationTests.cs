using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace ArrayApp.Application.IntegrationTests.Ideas;

using static Testing;

public class EnterpriseConnectorsIntegrationTests : BaseTestFixture
{
    [Test]
    public async Task ShouldConfigureConnectorAndSyncActions()
    {
        await RunAsDefaultUserAsync();

        // 1. Create Idea
        var ideaId = await SendAsync(new CreateIdeaCommand
        {
            Title = "Autonomous Fleet Routing Mesh",
            Description = "Dynamic routing engine for zero-emission delivery fleets",
            Content = "Specification document for fleet telemetry dispatch",
            CategoryId = 1
        });

        // 2. Configure Jira Connector
        var configDto = await SendAsync(new ConfigureConnectorCommand
        {
            IdeaId = ideaId,
            Type = ConnectorType.Jira,
            Name = "Fleet Jira Sprint Board",
            TargetEndpoint = "https://arrayapp.atlassian.net/rest/api/3",
            ApiKey = "sec_test_api_token_jira_123",
            ProjectOrChannelKey = "FLEET",
            AutoSyncActions = true,
            ActorName = "DevOps Lead"
        });

        configDto.Should().NotBeNull();
        configDto.Name.Should().Be("Fleet Jira Sprint Board");
        configDto.IsActive.Should().BeTrue();

        // 3. Create an Action Item on this Idea
        var idea = await FindAsync<Idea>(ideaId);
        idea.Should().NotBeNull();
        var actionItem = new ActionItem
        {
            IdeaId = ideaId,
            Title = "Provision GPS Gateway",
            Description = "Configure cloud ingress for OBD-II vehicle telemetry",
            OwnerUserId = "engineer-1",
            Priority = ActionItemPriority.High,
            Status = ActionItemStatus.Todo
        };
        await AddAsync(actionItem);

        // 4. Sync Action Item to Jira
        var syncResult = await SendAsync(new SyncActionToConnectorCommand
        {
            ActionId = actionItem.Id,
            ConnectorType = ConnectorType.Jira,
            ActorName = "DevOps Lead"
        });

        syncResult.Should().NotBeNull();
        syncResult.ActionItemId.Should().Be(actionItem.Id);
        syncResult.IsSuccess.Should().BeTrue();
        syncResult.ExternalTicketId.Should().NotBeNullOrWhiteSpace();

        // 5. Inbound Webhook Reconciliation
        var webhookResult = await SendAsync(new ProcessInboundWebhookCommand
        {
            ExternalSystem = "Jira",
            ExternalReferenceKey = syncResult.ExternalTicketId!,
            Status = "Done",
            ResolutionComment = "Resolved via remote sprint completion"
        });

        webhookResult.Should().BeTrue();
    }
}
