using Core.Models;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using NotificationService.Hubs;

namespace NotificationService.Consumers;

public class MetricsNotificationConsumer : IConsumer<Metrics>
{
    private readonly IHubContext<MetricsHub> _hubContext;
    private readonly ILogger<MetricsNotificationConsumer> _logger;

    public MetricsNotificationConsumer(IHubContext<MetricsHub> hubContext, ILogger<MetricsNotificationConsumer> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<Metrics> context)
    {
        var message = context.Message;

        try
        {
            await _hubContext.Clients.All.SendAsync("MetricReceived", new
            {
                message.Type,
                message.Name,
                message.Payload,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogInformation("Metric forwarded to clients: {Type} - {Name}", message.Type, message.Name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to forward metric to clients: {Type} - {Name}", message.Type, message.Name);
            throw;
        }
    }
}

