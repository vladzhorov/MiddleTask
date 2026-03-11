using Core.Models;

namespace DataProccesor.Repositories.Interfaces
{
    public interface IMetricRepository
    {
        public Task InsertMetricAsync(MetricDocument document);
    }
}
