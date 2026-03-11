using Core.Models;
using DataIngestorService.Services.Interfaces;
using FluentValidation;
using MassTransit;
using Quartz;
using Microsoft.Extensions.Logging;

namespace DataIngestorService.BackgroundJob
{
    public class FetchApiJob : IJob
    {
        private readonly IWeakApiService _weakApiService;

        private readonly IValidator<Metrics> _validator;

        private readonly IPublishEndpoint _publishEndpoint;

        private readonly ILogger<FetchApiJob> _logger;

        public FetchApiJob(
            IWeakApiService weakApiService,
            IValidator<Metrics> validator,
            IPublishEndpoint publushEndpoint,
            ILogger<FetchApiJob> logger)
        {
            _weakApiService = weakApiService;
            _validator = validator;
            _publishEndpoint = publushEndpoint;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var data = await _weakApiService.GetMetricsFromApi();

            if (data == null) { return; }

            var validator = _validator.Validate(data);

            if (validator.IsValid)
            {
                await _publishEndpoint.Publish<Metrics>(data, ctx =>
                {
                    ctx.SetRoutingKey(data.Type.ToLower());
                });
            }
            else
            {
                _logger.LogWarning("Invalid data from API");
            }
        }
    }
}