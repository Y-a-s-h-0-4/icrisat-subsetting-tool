using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace icrisat_subsetting.Migrations
{
    /// <inheritdoc />
    public partial class AddWeatherDataColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "charecterstics",
                schema: "dbo",
                columns: table => new
                {
                    ICRISAT_accession_identifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    Humidity = table.Column<double>(type: "float", nullable: false),
                    Rainfall = table.Column<double>(type: "float", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_charecterstics", x => x.ICRISAT_accession_identifier);
                });

            migrationBuilder.CreateTable(
                name: "passport_data",
                schema: "dbo",
                columns: table => new
                {
                    ICRISAT_accession_identifier = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Latitude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Longitude = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_passport_data", x => x.ICRISAT_accession_identifier);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "charecterstics",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "passport_data",
                schema: "dbo");
        }
    }
}
