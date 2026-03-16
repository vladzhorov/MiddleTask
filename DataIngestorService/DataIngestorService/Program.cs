using DataIngestorService.Services;
using DataIngestorService.Services.Interfaces;
using DataIngestorService.BackgroundJob;
using DataIngestorService.Extensions;
using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.Configuration;
using Quartz;
using Polly.Extensions.Http;
using Polly;
using Core.Models;
using FluentValidation;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build())
    .WriteTo.Console()
    .CreateLogger();

var builder = Host.CreateApplicationBuilder();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger, dispose: true);

var retryPolicy = HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

builder.Services.Configure<WeakApiOptions>(builder.Configuration.GetSection("WeakApi"));
builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMq"));

builder.Services.AddHttpClient<IWeakApiService, WeakApIService>()
    .AddPolicyHandler(retryPolicy);

builder.Services.AddScoped<IValidator<Metrics>, MetricsValidator>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, config) =>
    {
        var rabbitOptions = context.GetRequiredService<Microsoft.Extensions.Options.IOptions<RabbitMqOptions>>().Value;

        x.SetKebabCaseEndpointNameFormatter();
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

builder.Services.AddQuartz(quar =>
{
    quar.AddCronJob<FetchApiJob>(builder.Configuration, "Quartz:FetchApiJob");
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var host = builder.Build();
await host.RunAsync();

