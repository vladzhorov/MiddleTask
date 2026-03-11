
using System.Text.Json.Serialization;

namespace Core.Models
{
    public class Metrics
    {
        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("payload")]
        public Payload Payload { get; set; }
    }
}
