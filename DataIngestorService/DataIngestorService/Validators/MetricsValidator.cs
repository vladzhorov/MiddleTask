using Core.Models;
using Core.Constants;
using FluentValidation;

namespace DataIngestorService.Validators;

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
            When(x => x.Type == MetricTypes.AirQuality, () =>
            {
                RuleFor(x => x.Payload.co2).NotNull().GreaterThanOrEqualTo(0);
                RuleFor(x => x.Payload.pm25).NotNull().GreaterThanOrEqualTo(0);
                RuleFor(x => x.Payload.humidity).NotNull().InclusiveBetween(0, 100);
            });

            When(x => x.Type == MetricTypes.Energy, () =>
            {
                RuleFor(x => x.Payload.energy).NotNull().GreaterThanOrEqualTo(0);
            });

            When(x => x.Type == MetricTypes.Motion, () =>
            {
                RuleFor(x => x.Payload.motionDetected).NotNull();
            });
        });
    }
}

