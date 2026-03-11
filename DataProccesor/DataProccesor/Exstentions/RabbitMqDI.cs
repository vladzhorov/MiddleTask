using Core.Models;
using DataProccesor.Сonsumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataProccesor.Exstentions
{
    public static class RabbitMqDI
    {
        public static void AddRabbitMQDependensies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMassTransit(x =>
            {
                x.AddConsumer<MetricConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host("localhost", h => { });

                    cfg.ReceiveEndpoint("processor-metrics-queue", e =>
                    {
                        e.Bind<Metrics>(exchange =>
                        {
                            exchange.ExchangeType = "topic";
                            exchange.RoutingKey = "#";
                        });

                        e.ConfigureConsumer<MetricConsumer>(context);
                    });
                });
            });
        }
    }
}