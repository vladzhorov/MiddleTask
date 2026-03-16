using API.Models;
using Core.Models;
using MongoDB.Driver;

namespace API.Repositories;

public class MetricReadRepository : IMetricReadRepository
{
    private readonly IMongoCollection<MetricDocument> _collection;

    public MetricReadRepository(IMongoCollection<MetricDocument> collection)
        => _collection = collection;

    public IQueryable<MetricDocument> GetMetrics()
    {
        return _collection.AsQueryable();
    }

    public async Task<MetricDocument?> GetByIdAsync(string id)
    {
        var filter = Builders<MetricDocument>.Filter.Eq(d => d.Id, id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<MetricTypeAggregation>> GetAggregationsByTypeAsync()
    {
        var pipeline = _collection.Aggregate()
            .Group(
                x => x.Type,
                g => new MetricTypeAggregation
                {
                    Type = g.Key,
                    Count = g.Count(),
                    AvgEnergy = g.Average(m => m.Payload.energy)
                });

        return await pipeline.ToListAsync();
    }
}

