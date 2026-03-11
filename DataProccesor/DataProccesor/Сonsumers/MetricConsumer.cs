using Core.Models;
using DataProccesor.Repositories.Interfaces;
using MassTransit;

namespace DataProccesor.Сonsumers
{
    public class MetricConsumer : IConsumer<Metrics>
    {
        private readonly IMetricRepository _metricRepository;
        public MetricConsumer(IMetricRepository metricRepository) => _metricRepository = metricRepository;

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

            await _metricRepository.InsertMetricAsync(document);
        }
    }
}
