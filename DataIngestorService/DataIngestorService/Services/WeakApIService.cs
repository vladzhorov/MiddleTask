using Core.Models;
using DataIngestorService.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace DataIngestorService.Services
{
    public class WeakApIService : IWeakApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IOptions<WeakApiOptions> _weakApiOptions;

        public WeakApIService(HttpClient httpClient, IOptions<WeakApiOptions> weakApiOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _weakApiOptions = weakApiOptions ?? throw new ArgumentNullException(nameof(weakApiOptions));
        }

        public async Task<Metrics> GetMetricsFromApi()
        {
            try
            {
                var apiUrl = _weakApiOptions.Value.Url;
                return await _httpClient.GetFromJsonAsync<Metrics>(apiUrl);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching metrics: {ex.Message}");
                return null;
            }
        }
    }
}
