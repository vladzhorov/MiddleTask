using FluentValidation;

namespace Core.Models;

public class MetricsValidator : AbstractValidator<Metrics>
{
    public MetricsValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.Payload)
            .NotNull();

        When(x => x.Payload is not null, () =>
        {
            RuleFor(x => x.Payload.co2)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Payload.pm25)
                .GreaterThanOrEqualTo(0);

            RuleFor(x => x.Payload.humidity)
                .InclusiveBetween(0, 100);

            RuleFor(x => x.Payload.energy)
                .GreaterThanOrEqualTo(0);
        });
    }
}

