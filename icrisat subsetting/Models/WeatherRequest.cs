namespace WeatherAPI.Models
{
    public class WeatherRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long StartTime { get; set; } // Unix timestamp in seconds
        public long EndTime { get; set; }   // Unix timestamp in seconds
    }
}
