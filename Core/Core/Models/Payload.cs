using System.Text.Json.Serialization;

namespace Core.Models
{
    public class Payload
    {
        [JsonPropertyName("co2")]
        public int co2 { get; set; }

        [JsonPropertyName("pm25")]
        public int pm25 { get; set; }

        [JsonPropertyName("humidity")]
        public int humidity { get; set; }

        [JsonPropertyName("energy")]
        public double energy { get; set; }
    }
}