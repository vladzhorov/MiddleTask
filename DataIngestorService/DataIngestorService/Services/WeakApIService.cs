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
                var apiKey = _weakApiOptions.Value.ApiKey;

                using var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("X-Api-Key", apiKey);

                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var metrics = await response.Content.ReadFromJsonAsync<Metrics>();
                return metrics;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching metrics: {ex.Message}");
                return null;
            }
        }
    }
}
