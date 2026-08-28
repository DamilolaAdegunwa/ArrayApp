using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Domain.Enums;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class RecordIdeaOutcomeCommandTests
{
    [Test]
    public void RecordIdeaOutcomeCommand_Initialization_CostSavingsCalculated()
    {
        var command = new RecordIdeaOutcomeCommand
        {
            IdeaId = 75,
            Title = "Edge IoT Logistics Automation Launch",
            Summary = "Deployed across 12 distribution facilities",
            Type = OutcomeType.BusinessImpact,
            EstimatedCostSavings = 350000.0,
            RevenueGenerated = 150000.0,
            ImpactedUsersCount = 12000,
            EstimatedRoiPercent = 210.0,
            ActorName = "VP of Supply Chain"
        };

        command.IdeaId.Should().Be(75);
        command.Title.Should().Be("Edge IoT Logistics Automation Launch");
        command.EstimatedCostSavings.Should().Be(350000.0);
        command.EstimatedRoiPercent.Should().Be(210.0);
    }
}
