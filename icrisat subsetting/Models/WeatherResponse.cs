using System.Text.Json.Serialization;

namespace WeatherAPI.Models
{
    public class WeatherResponse
    {
        public string ICRISAT_accession_identifier { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Rainfall { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int DataPoints { get; set; }
    }
} 