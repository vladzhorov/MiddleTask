using DataProccesor.Models;
using DataProccesor.Repositories;
using DataProccesor.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace DataProccesor.Exstentions
{
    public static class MongoDI
    {
        public static void AddMongoDependensies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IMetricRepository, MetricRepository>();

            services.Configure<MongoDbSettings>(options => configuration.GetSection(nameof(MongoDbSettings)).Bind(options));

            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

                if (settings == null || string.IsNullOrEmpty(settings.ConnectionString))
                {
                    throw new ArgumentNullException("MongoDB ConnectionString is missing in appsettings.json");
                }

                return new MongoClient(settings.ConnectionString);
            });
        }
    }
}
