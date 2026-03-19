using Core.Models;
using API.Models;

namespace API.Repositories;

public interface IMetricReadRepository
{
    IQueryable<MetricDocument> GetMetrics();
    Task<IReadOnlyList<MetricDocument>> GetMetricsPageAsync(int skip, int take);
    Task<MetricDocument?> GetByIdAsync(string id);
    Task<IReadOnlyList<MetricTypeAggregation>> GetAggregationsByTypeAsync();
}

