using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherAPI.Models
{
    [Table("charecterstics", Schema = "dbo")]
    public class Characteristics
    {
        [Key]
        public string ICRISAT_accession_identifier { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Rainfall { get; set; }
        public DateTime Timestamp { get; set; }
    }
} 