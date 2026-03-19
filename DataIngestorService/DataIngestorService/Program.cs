using DataIngestorService.Services;
using DataIngestorService.Services.Interfaces;
using DataIngestorService.BackgroundJob;
using DataIngestorService.Extensions;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Polly.Extensions.Http;
using Polly;
using Core.Models;
using FluentValidation;
using Serilog;
using DataIngestorService.Validators;

var builder = Host.CreateApplicationBuilder();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger, dispose: true);

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

builder.Services.Configure<WeakApiOptions>(options =>
    builder.Configuration.GetSection(nameof(WeakApiOptions)).Bind(options));
builder.Services.Configure<RabbitMqOptions>(options =>
    builder.Configuration.GetSection(nameof(RabbitMqOptions)).Bind(options));

builder.Services.AddHttpClient<IWeakApiService, WeakApIService>((sp, client) =>
    {
        var weakApiOptions = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<WeakApiOptions>>().Value;

        client.BaseAddress = new Uri(weakApiOptions.Url);

        if (!string.IsNullOrWhiteSpace(weakApiOptions.ApiKey))
        {
            client.DefaultRequestHeaders.Remove("X-Api-Key");
            client.DefaultRequestHeaders.Add("X-Api-Key", weakApiOptions.ApiKey);
        }
    })
    .AddPolicyHandler(retryPolicy);

builder.Services.AddScoped<IValidator<Metrics>, MetricsValidator>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingRabbitMq((context, config) =>
    {
        var rabbitOptions = context.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqOptions>>().Value;

        config.Host(new Uri($"rabbitmq://{rabbitOptions.Host}"), h =>
        {
            h.Username(rabbitOptions.UserName);
            h.Password(rabbitOptions.Password);
        });
        config.Publish<Metrics>(p =>
        {
            p.ExchangeType = "topic";
        });
    });
});

builder.Services.AddConfiguredQuartzScheduler(builder.Configuration);

var host = builder.Build();
await host.RunAsync();

