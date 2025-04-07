using System.ComponentModel.DataAnnotations;

namespace WeatherAPI.Models
{
    public class WeatherRequest
    {
        [Required]
        public string ICRISAT_accession_identifier { get; set; }

        [Required]
        public long StartTime { get; set; }

        public long? EndTime { get; set; }
    }
}
