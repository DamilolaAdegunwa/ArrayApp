using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using ArrayApp.Domain.Entities.CategoryAggregate;
using ArrayApp.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace ArrayApp.Application.IntegrationTests.Ideas;

using static Testing;

public class AIAgentSwarmIntegrationTests : BaseTestFixture
{
    [Test]
    public async Task ShouldInvokeAIAgentAndPinInsight()
    {
        await RunAsDefaultUserAsync();

        // 1. Create Idea
        var ideaId = await SendAsync(new CreateIdeaCommand
        {
            Title = "Autonomous Greenhouse Micro-Climate Controller",
            Description = "Automated sensor mesh regulating moisture and nutrients",
            Content = "System design specification for indoor commercial micro-farming",
            CategoryId = 1
        });

        // 2. Invoke Critic (Red Team) AI Agent
        var insightDto = await SendAsync(new InvokeAIAgentCommand
        {
            IdeaId = ideaId,
            AgentType = AIAgentType.Critic,
            CustomPrompt = "Stress test lithium ion power grid in continuous 98% humidity conditions."
        });

        insightDto.Should().NotBeNull();
        insightDto.IdeaId.Should().Be(ideaId);
        insightDto.AgentType.Should().Be(AIAgentType.Critic);
        insightDto.ConfidenceScore.Should().BeGreaterThan(0.0);
        insightDto.IsPinned.Should().BeFalse();

        var pinSuccess = await SendAsync(new PinAIAgentInsightCommand(insightDto.Id));

        pinSuccess.Should().BeTrue();

        // 4. Query Insights for Idea
        var insights = await SendAsync(new GetAIAgentInsightsQuery(ideaId));
        insights.Should().NotBeEmpty();
        insights.Should().Contain(i => i.Id == insightDto.Id && i.IsPinned);
    }
}
