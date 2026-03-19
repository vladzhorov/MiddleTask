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
            var metrics = await FetchAsync();
            if (metrics is null || metrics.Count == 0)
                return;

            var published = 0;
            var invalid = 0;

            foreach (var metric in metrics)
            {
                if (metric is null)
                    continue;

                if (!IsValid(metric))
                {
                    invalid++;
                    continue;
                }

                if (await TryPublishAsync(metric))
                    published++;
            }

            if (published > 0 || invalid > 0)
            {
                _logger.LogInformation("FetchApiJob finished. Published={Published}, Invalid={Invalid}", published, invalid);
            }
        }

        private async Task<IReadOnlyList<Metrics>?> FetchAsync()
        {
            try
            {
                return await _weakApiService.GetMetricsFromApi();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception while fetching metrics");
                return null;
            }
        }

        private bool IsValid(Metrics metric)
        {
            var result = _validator.Validate(metric);
            if (result.IsValid)
                return true;

            _logger.LogWarning(
                "Invalid metric from API. Type={Type}, Name={Name}",
                metric.Type,
                metric.Name);

            return false;
        }

        private async Task<bool> TryPublishAsync(Metrics metric)
        {
            try
            {
                await _publishEndpoint.Publish<Metrics>(metric, ctx =>
                {
                    ctx.SetRoutingKey(metric.Type.ToLowerInvariant());
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish metric to RabbitMQ");
                return false;
            }
        }
    }
}