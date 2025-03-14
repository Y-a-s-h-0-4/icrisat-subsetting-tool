using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WeatherAPI.Models
{
    public class WeatherApiResponse
    {
        [JsonPropertyName("list")]
        public List<WeatherEntry> List { get; set; }
    }

    public class WeatherEntry
    {
        [JsonPropertyName("main")]
        public MainData Main { get; set; }
    }

    public class MainData
    {
        [JsonPropertyName("temp")]
        public double Temperature { get; set; }

        [JsonPropertyName("humidity")]
        public double Humidity { get; set; }

        [JsonPropertyName("pressure")]
        public double Pressure { get; set; }
    }
}
