using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontendApp.Models
{
    [Table("passport_data")]
    public class PassportData
    {
        [Key]
        [Column("ICRISAT_accession_identifier")]
        public string ICRISATAccessionIdentifier { get; set; }

        [Column("Accession_identifier")]
        public string AccessionIdentifier { get; set; }

        [Column("Crop")]
        public string Crop { get; set; }

        [Column("DOI")]
        public string DOI { get; set; }

        [Column("Local_name")]
        public string LocalName { get; set; }

        [Column("Genus")]
        public string Genus { get; set; }

        [Column("Species")]
        public string Species { get; set; }
    }
}