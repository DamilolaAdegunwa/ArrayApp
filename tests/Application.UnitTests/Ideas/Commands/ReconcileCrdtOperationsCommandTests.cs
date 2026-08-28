using ArrayApp.Application.Ideas.Commands;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class ReconcileCrdtOperationsCommandTests
{
    [Test]
    public void ReconcileCrdtOperationsCommand_Initialization_VectorClockPreserved()
    {
        var command = new ReconcileCrdtOperationsCommand
        {
            IdeaId = 95,
            ClientId = "pwa-client-xyz",
            ClientVectorClock = new Dictionary<string, long>
            {
                { "pwa-client-xyz", 12 },
                { "server", 100 }
            },
            Operations = new List<CrdtOperationDto>
            {
                new()
                {
                    EntityType = "IdeaDimension",
                    FieldName = "ProblemStatement",
                    ValueJson = "\"Low battery retention during winter routes\"",
                    ClientSequence = 12
                }
            }
        };

        command.IdeaId.Should().Be(95);
        command.ClientId.Should().Be("pwa-client-xyz");
        command.Operations.Should().HaveCount(1);
        command.ClientVectorClock.Should().ContainKey("pwa-client-xyz");
    }

    [Test]
    public void CrdtReconciliationResultDto_SetsDefaults()
    {
        var dto = new CrdtReconciliationResultDto
        {
            IdeaId = 95,
            ClientId = "pwa-client-xyz",
            OperationsApplied = 1,
            ConflictResolved = true
        };

        dto.ConflictResolved.Should().BeTrue();
        dto.OperationsApplied.Should().Be(1);
    }
}
