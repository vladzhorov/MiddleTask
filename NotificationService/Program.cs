using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using NotificationService.Extensions;
using NotificationService.Hubs;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build())
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(Log.Logger, dispose: true);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true));
});

builder.Services.AddSignalR();

builder.Services.AddRabbitMqDependencies(builder.Configuration);

var app = builder.Build();

app.UseCors();

app.MapHub<MetricsHub>("/hubs/metrics");

app.Run();

