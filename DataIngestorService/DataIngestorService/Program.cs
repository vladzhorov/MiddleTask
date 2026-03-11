using DataIngestorService.Services;
using DataIngestorService.Services.Interfaces;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection; 
using Quartz;
using Polly.Extensions.Http;
using Polly;
using Core.Models;

var builder = Host.CreateApplicationBuilder();

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

builder.Services.AddHttpClient<IWeakApiService, WeakApIService>()
    .AddPolicyHandler(retryPolicy);

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, config) =>
    {
        x.SetKebabCaseEndpointNameFormatter();
        config.Host(new Uri("rabbitmq://localhost"), h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        config.Publish<Metrics>(p =>
        {
            p.ExchangeType = "topic";
        });
    });
});

builder.Services.AddQuartz(quar => { });

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var host = builder.Build();
await host.RunAsync();

