using Core.Models;
using DataProccesor.Repositories.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DataProccesor.Сonsumers
{
    public class MetricConsumer : IConsumer<Metrics>
    {
        private readonly IMetricRepository _metricRepository;
        private readonly ILogger<MetricConsumer> _logger;

        public MetricConsumer(IMetricRepository metricRepository, ILogger<MetricConsumer> logger)
        {
            _metricRepository = metricRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<Metrics> context)
        {
            var message = context.Message;

            var document = new MetricDocument
            {
                Type = message.Type,
                Name = message.Name,
                Payload = message.Payload,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _metricRepository.InsertMetricAsync(document);
                _logger.LogInformation("Metric persisted: {Type} - {Name}", document.Type, document.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to persist metric: {Type} - {Name}", document.Type, document.Name);
                throw;
            }
        }
    }
}
