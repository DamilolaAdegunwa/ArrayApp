using ArrayApp.Application.Ideas.Commands;
using ArrayApp.Application.Ideas.Queries;
using ArrayApp.Domain.Entities.IdeaAggregate;
using ArrayApp.Domain.Enums;
using FluentAssertions;
using NUnit.Framework;

namespace ArrayApp.Application.IntegrationTests.Ideas;

using static Testing;

public class ZeroTrustAndEdgeSyncIntegrationTests : BaseTestFixture
{
    [Test]
    public async Task ShouldEvaluateAbacAndReconcileCrdtOperations()
    {
        await RunAsDefaultUserAsync();

        // 1. Create Idea
        var ideaId = await SendAsync(new CreateIdeaCommand
        {
            Title = "Quantum Resilient Mesh Protocol",
            Description = "Lattice-based encryption for edge sensor clusters",
            Content = "Post-quantum key encapsulation mechanism architecture",
            CategoryId = 1
        });

        // 2. Evaluate Dynamic ABAC Access
        var abacResult = await SendAsync(new EvaluateIdeaAccessQuery(
            ideaId,
            "engineer-sec-1",
            "Engineering",
            "TopSecret"
        ));

        abacResult.Should().NotBeNull();
        abacResult.IsAllowed.Should().BeTrue();

        // 3. Reconcile Offline CRDT Operations
        var crdtCommand = new ReconcileCrdtOperationsCommand
        {
            IdeaId = ideaId,
            ClientId = "field-device-iphone-alpha",
            Operations = new List<CrdtOperationDto>
            {
                new CrdtOperationDto
                {
                    ClientId = "field-device-iphone-alpha",
                    ClientSequence = 1,
                    EntityType = "IdeaDimension",
                    EntityId = ideaId.ToString(),
                    FieldName = "Constraints",
                    ValueJson = "\"Offline battery conservation limits key exchange to once per hour\"",
                    OperationType = "Update",
                    Timestamp = DateTimeOffset.UtcNow
                }
            },
            ClientVectorClock = new Dictionary<string, long>
            {
                { "field-device-iphone-alpha", 1 }
            }
        };

        var reconcileResult = await SendAsync(crdtCommand);

        reconcileResult.Should().NotBeNull();
        reconcileResult.IdeaId.Should().Be(ideaId);
        reconcileResult.OperationsApplied.Should().Be(1);
        reconcileResult.ConflictResolved.Should().BeTrue();
        reconcileResult.ServerSequence.Should().BeGreaterThan(0);
        reconcileResult.ServerVectorClock.Should().ContainKey("field-device-iphone-alpha");
    }
}
