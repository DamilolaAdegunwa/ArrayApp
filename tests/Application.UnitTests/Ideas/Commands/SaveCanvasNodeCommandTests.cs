using ArrayApp.Application.Ideas.Commands;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class SaveCanvasNodeCommandTests
{
    [Test]
    public void SaveCanvasNodeCommand_StickyNote_SetsCoordinates()
    {
        var command = new SaveCanvasNodeCommand
        {
            IdeaId = 20,
            SessionId = 5,
            NodeType = "Sticky",
            Content = "Need to test high-altitude battery degradation",
            PosX = 250.5,
            PosY = 180.0,
            ColorHex = "#FEF08A",
            AuthorName = "Dr. Elena Vance"
        };

        command.IdeaId.Should().Be(20);
        command.SessionId.Should().Be(5);
        command.NodeType.Should().Be("Sticky");
        command.PosX.Should().Be(250.5);
        command.PosY.Should().Be(180.0);
        command.AuthorName.Should().Be("Dr. Elena Vance");
    }

    [Test]
    public void VoteCanvasNodeCommand_SetsIncrement()
    {
        var command = new VoteCanvasNodeCommand(20, 101, 2);
        command.IdeaId.Should().Be(20);
        command.NodeId.Should().Be(101);
        command.Increment.Should().Be(2);
    }
}
