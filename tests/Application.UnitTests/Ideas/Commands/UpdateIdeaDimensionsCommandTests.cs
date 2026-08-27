using ArrayApp.Application.Ideas.Commands;

namespace ArrayApp.Application.UnitTests.Ideas.Commands;

[TestFixture]
public class UpdateIdeaDimensionsCommandTests
{
    private UpdateIdeaDimensionsCommandValidator _validator;

    [SetUp]
    public void SetUp()
    {
        _validator = new UpdateIdeaDimensionsCommandValidator();
    }

    [Test]
    public void ShouldValidate_ValidCommand_ReturnsTrue()
    {
        var command = new UpdateIdeaDimensionsCommand
        {
            IdeaId = 1,
            ProblemStatement = "Inefficient manual workflows",
            Opportunity = "$500k annual operational savings",
            Hypothesis = "If we automate workflow X, cycle time drops by 40%",
            ImpactScore = 8.5,
            ConfidenceScore = 8.0,
            EaseScore = 7.0,
            ReachScore = 1000,
            EffortScore = 2.0
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Test]
    public void ShouldFailValidation_InvalidScores_ReturnsValidationErrors()
    {
        var command = new UpdateIdeaDimensionsCommand
        {
            IdeaId = 0, // Invalid
            ImpactScore = 15.0, // Invalid > 10
            ConfidenceScore = 0.0, // Invalid < 1
            EaseScore = -2.0, // Invalid < 1
            EffortScore = 0.0 // Invalid <= 0
        };

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterOrEqualTo(4);
    }
}
