namespace ArrayApp.Application.Ideas.Commands;

public class UpdateIdeaDimensionsCommandValidator : AbstractValidator<UpdateIdeaDimensionsCommand>
{
    public UpdateIdeaDimensionsCommandValidator()
    {
        RuleFor(v => v.IdeaId)
            .GreaterThan(0)
            .WithMessage("IdeaId must be greater than 0.");

        RuleFor(v => v.ImpactScore)
            .InclusiveBetween(1.0, 10.0)
            .WithMessage("ImpactScore must be between 1.0 and 10.0.");

        RuleFor(v => v.ConfidenceScore)
            .InclusiveBetween(1.0, 10.0)
            .WithMessage("ConfidenceScore must be between 1.0 and 10.0.");

        RuleFor(v => v.EaseScore)
            .InclusiveBetween(1.0, 10.0)
            .WithMessage("EaseScore must be between 1.0 and 10.0.");

        RuleFor(v => v.EffortScore)
            .GreaterThan(0.0)
            .WithMessage("EffortScore must be greater than 0.");
    }
}
