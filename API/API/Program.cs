using API.GraphQL;
using API.Repositories;
using API.Extensions;
using Serilog;
using Microsoft.Extensions.Configuration;

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

builder.Services.AddMongoDependencies(builder.Configuration);

builder.Services.AddScoped<IMetricReadRepository, MetricReadRepository>();

builder.Services.AddControllers();

// GraphQL configuration
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddFiltering()
    .AddSorting()
    .AddProjections();

var app = builder.Build();

app.UseCors();

app.MapControllers();
app.MapGraphQL("/graphql");

app.Run();
