using Core.Models;
using DataIngestorService.Services.Interfaces;
using System.Net.Http.Json;

namespace DataIngestorService.Services
{
    public class WeakApIService : IWeakApiService
    {
        private readonly HttpClient _httpClient;

        public WeakApIService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<IReadOnlyList<Metrics>?> GetMetricsFromApi()
        {
            try
            {
                // BaseAddress and auth headers are configured in Program.cs via AddHttpClient(...)
                return await _httpClient.GetFromJsonAsync<List<Metrics>>(string.Empty);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching metrics: {ex.Message}");
                return null;
            }
            catch (System.Text.Json.JsonException ex)
            {
                Console.WriteLine($"Error parsing metrics JSON: {ex.Message}");
                return null;
            }
        }
    }
}
