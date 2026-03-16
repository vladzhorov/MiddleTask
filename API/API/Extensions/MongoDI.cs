using API.Models;
using Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace API.Extensions;

public static class MongoDI
{
    public static void AddMongoDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;

            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
            {
                throw new InvalidOperationException(
                    "MongoDbSettings:ConnectionString is not configured for API service.");
            }

            return new MongoClient(settings.ConnectionString);
        });

        services.AddScoped<IMongoCollection<MetricDocument>>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var client = sp.GetRequiredService<IMongoClient>();
            var database = client.GetDatabase(settings.DatabaseName);
            return database.GetCollection<MetricDocument>(settings.CollectionName);
        });
    }
}

