using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace WeatherAPI.Data.Migrations
{
    public partial class AddWeatherColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Temperature column
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Temperature')
                BEGIN
                    ALTER TABLE [dbo].[charecterstics] ADD Temperature float NOT NULL DEFAULT 0.0;
                END");

            // Add Humidity column
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Humidity')
                BEGIN
                    ALTER TABLE [dbo].[charecterstics] ADD Humidity float NOT NULL DEFAULT 0.0;
                END");

            // Add Rainfall column
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Rainfall')
                BEGIN
                    ALTER TABLE [dbo].[charecterstics] ADD Rainfall float NOT NULL DEFAULT 0.0;
                END");

            // Add Timestamp column
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Timestamp')
                BEGIN
                    ALTER TABLE [dbo].[charecterstics] ADD Timestamp datetime2 NOT NULL DEFAULT GETUTCDATE();
                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Temperature')
                BEGIN
                    ALTER TABLE [dbo].[charecterstics] DROP COLUMN Temperature;
                END");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Humidity')
                BEGIN
                    ALTER TABLE [dbo].[charecterstics] DROP COLUMN Humidity;
                END");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Rainfall')
                BEGIN
                    ALTER TABLE [dbo].[charecterstics] DROP COLUMN Rainfall;
                END");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Timestamp')
                BEGIN
                    ALTER TABLE [dbo].[charecterstics] DROP COLUMN Timestamp;
                END");
        }
    }
} 