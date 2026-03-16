using Core.Models;
using DataProccesor.Сonsumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataProccesor.Exstentions
{
    public static class RabbitMqDI
    {
        public static void AddRabbitMQDependensies(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("RabbitMq"));

            services.AddMassTransit(x =>
            {
                x.AddConsumer<MetricConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    var rabbitOptions = context
                        .GetRequiredService<IOptions<RabbitMqOptions>>()
                        .Value;

                    cfg.Host(rabbitOptions.Host, h =>
                    {
                        h.Username(rabbitOptions.UserName);
                        h.Password(rabbitOptions.Password);
                    });

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