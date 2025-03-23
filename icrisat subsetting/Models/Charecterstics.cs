public class Characteristics
{
    public int Id { get; set; }
    public string ICRISAT_accession_identifier { get; set; } // Foreign Key
    public double? Temperature { get; set; }
    public double? Precipitation { get; set; }
    public double? Rainfall { get; set; }
}
