using Core.Models;
using DataProccesor.Models;
using DataProccesor.Repositories.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace DataProccesor.Repositories
{
    public class MetricRepository : IMetricRepository
    {
        private readonly IMongoClient _mongoClient;

        private readonly IOptions<MongoDbSettings> _mongoOptions;

        private readonly IMongoCollection<MetricDocument> _collection;

        public MetricRepository(IMongoClient mongoClient, IOptions<MongoDbSettings> mongoOptions)
        {
            _mongoClient = mongoClient;
            _mongoOptions = mongoOptions;

            var database = _mongoClient.GetDatabase(_mongoOptions.Value.DatabaseName);
            _collection = database.GetCollection<MetricDocument>(_mongoOptions.Value.CollectionName);

        }

        public async Task InsertMetricAsync(MetricDocument document)
        {
            await _collection.InsertOneAsync(document);
        }
    }
}
