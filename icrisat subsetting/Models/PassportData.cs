using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherAPI.Models
{
    [Table("passport_data", Schema = "dbo")]
    public class PassportData
    {
        [Key]
        public string ICRISAT_accession_identifier { get; set; } = string.Empty;
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
    }
} 