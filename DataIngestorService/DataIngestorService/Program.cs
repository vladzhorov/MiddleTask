using DataIngestorService.Services;
using DataIngestorService.Services.Interfaces;
using DataIngestorService.BackgroundJob;
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

builder.Services.AddQuartz(quar =>
{
    var jobSection = builder.Configuration.GetSection("Quartz:FetchApiJob").Get<CronJobOptions>();

    if (jobSection is null || string.IsNullOrWhiteSpace(jobSection.Schedule))
    {
        throw new InvalidOperationException("Quartz:FetchApiJob schedule is not configured.");
    }

    var jobKey = new JobKey(nameof(FetchApiJob));

    quar.AddJob<FetchApiJob>(opts =>
        opts.WithIdentity(jobKey)
            .WithDescription(jobSection.Description ?? nameof(FetchApiJob)));

    quar.AddTrigger(opts =>
        opts.ForJob(jobKey)
            .WithIdentity($"{nameof(FetchApiJob)}-trigger")
            .WithCronSchedule(jobSection.Schedule));
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

var host = builder.Build();
await host.RunAsync();

