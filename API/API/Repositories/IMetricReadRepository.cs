using Core.Models;
using API.Models;

namespace API.Repositories;

public interface IMetricReadRepository
{
    IQueryable<MetricDocument> GetMetrics();
    Task<MetricDocument?> GetByIdAsync(string id);
    Task<IReadOnlyList<MetricTypeAggregation>> GetAggregationsByTypeAsync();
}

