using Core.Models;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NotificationService.Consumers;

namespace NotificationService.Extensions;

public static class RabbitMqDI
{
    public static void AddRabbitMqDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(options =>
            configuration.GetSection(nameof(RabbitMqOptions)).Bind(options));

        services.AddMassTransit(x =>
        {
            x.AddConsumer<MetricsNotificationConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var rabbitOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

                cfg.Host(rabbitOptions.Host, h =>
                {
                    h.Username(rabbitOptions.UserName);
                    h.Password(rabbitOptions.Password);
                });

                cfg.Publish<Metrics>(p =>
                {
                    p.ExchangeType = "topic";
                });

                cfg.ReceiveEndpoint("notifications-metrics-queue", e =>
                {
                    e.Bind<Metrics>(exchange =>
                    {
                        exchange.ExchangeType = "topic";
                        exchange.RoutingKey = "#";
                    });

                    e.ConfigureConsumer<MetricsNotificationConsumer>(context);
                });
            });
        });
    }
}

