using System.ComponentModel.DataAnnotations.Schema;

namespace FrontendApp.Models
{
    [Table("charecterstics")] // Map to the correct table name
    public class Characterization
    {
        public int Id { get; set; } // Assuming there's an Id column as the primary key (you may need to adjust if the primary key is different)

        [Column("ICRISAT_accession_identifier")]
        public string ICRISATAccessionIdentifier { get; set; }

        [Column("Race")]
        public string Race { get; set; }

        [Column("Plant_height_cm_postrainy")]
        public float? PlantHeightCmPostrainy { get; set; }

        [Column("Plant_height_cm_rainy")]
        public float? PlantHeightCmRainy { get; set; }

        [Column("Plant_pigmentation")]
        public string PlantPigmentation { get; set; }

        [Column("Basal_tillers_number")]
        public int? BasalTillersNumber { get; set; }

        [Column("Nodal_tillering")]
        public string NodalTillering { get; set; }

        [Column("Midrib_color")]
        public string MidribColor { get; set; }

        [Column("Days_to_flowering_postrainy")]
        public int? DaysToFloweringPostrainy { get; set; }

        [Column("Days_to_flowering_rainy")]
        public int? DaysToFloweringRainy { get; set; }

        [Column("Panicle_exertion_cm")]
        public float? PanicleExertionCm { get; set; }

        [Column("Panicle_length_cm")]
        public float? PanicleLengthCm { get; set; }

        [Column("Panicle_width_cm")]
        public float? PanicleWidthCm { get; set; }

        [Column("Panicle_compactness_and_shape")]
        public string PanicleCompactnessAndShape { get; set; }

        [Column("Glume_color")]
        public string GlumeColor { get; set; }

        [Column("Glume_covering")]
        public string GlumeCovering { get; set; }

        [Column("Seed_color")]
        public string SeedColor { get; set; }

        [Column("Seed_lustre")]
        public string SeedLustre { get; set; }

        [Column("Seed_subcoat")]
        public string SeedSubcoat { get; set; }

        [Column("Seed_size_mm")]
        public float? SeedSizeMm { get; set; }

        [Column("_100_Seed_weight_g")]
        public float? SeedWeight100G { get; set; }

        [Column("Endosperm_texture")]
        public string EndospermTexture { get; set; }

        [Column("Thresability")]
        public string Thresability { get; set; }

        [Column("Shoot_fly_rainy")]
        public float? ShootFlyRainy { get; set; }

        [Column("Shoot_fly_postrainy")]
        public float? ShootFlyPostrainy { get; set; }

        [Column("Downy_mildew_field")]
        public float? DownyMildewField { get; set; }

        [Column("Downy_mildew_glasshouse")]
        public float? DownyMildewGlasshouse { get; set; }

        [Column("Stem_borer")]
        public float? StemBorer { get; set; }

        [Column("Anthracnose")]
        public float? Anthracnose { get; set; }

        [Column("Grain_mold")]
        public float? GrainMold { get; set; }

        [Column("Leaf_blight")]
        public float? LeafBlight { get; set; }

        [Column("Midge")]
        public float? Midge { get; set; }

        [Column("Headbug")]
        public float? Headbug { get; set; }

        [Column("Rust")]
        public float? Rust { get; set; }

        [Column("Strigol_control")]
        public float? StrigolControl { get; set; }

        [Column("Protein")]
        public float? Protein { get; set; }

        [Column("Lysine")]
        public float? Lysine { get; set; }

        [Column("Remarks")]
        public string Remarks { get; set; }

        [Column("Year_of_characterization")]
        public int? YearOfCharacterization { get; set; }

        [Column("Temperature")]
        public float? Temperature { get; set; }

        [Column("Humidity")]
        public float? Humidity { get; set; }

        [Column("Rainfall")]
        public float? Rainfall { get; set; }

        [Column("Timestamp")]
        public DateTime? Timestamp { get; set; }
    }
}
