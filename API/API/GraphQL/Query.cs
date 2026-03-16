using API.Models;
using API.Repositories;
using Core.Models;
using HotChocolate;
using HotChocolate.Data;

namespace API.GraphQL;

public class Query
{
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<MetricDocument> GetMetrics([Service] IMetricReadRepository repository)
        => repository.GetMetrics();

    public Task<MetricDocument?> GetMetricById(string id, [Service] IMetricReadRepository repository)
        => repository.GetByIdAsync(id);

    public Task<IReadOnlyList<MetricTypeAggregation>> GetMetricsByTypeAggregations(
        [Service] IMetricReadRepository repository)
        => repository.GetAggregationsByTypeAsync();
}

