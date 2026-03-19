using Microsoft.AspNetCore.SignalR;
using NotificationService.Extensions;
using NotificationService.Hubs;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.Console()
    .CreateLogger();

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

