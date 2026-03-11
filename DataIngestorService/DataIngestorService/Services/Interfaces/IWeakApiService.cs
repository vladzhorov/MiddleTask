
using Core.Models;

namespace DataIngestorService.Services.Interfaces
{
    public interface IWeakApiService
    {
        Task<Metrics> GetMetricsFromApi();
    }
}
