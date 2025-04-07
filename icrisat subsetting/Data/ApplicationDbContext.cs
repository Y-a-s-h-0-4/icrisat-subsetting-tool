using Microsoft.EntityFrameworkCore;
using WeatherAPI.Models;

namespace WeatherAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            // Ensure the columns exist in the database
            EnsureColumnsExist();
        }

        public DbSet<Characteristics> Characteristics { get; set; }
        public DbSet<PassportData> PassportData { get; set; }

        private void EnsureColumnsExist()
        {
            try
            {
                Database.ExecuteSqlRaw(@"
                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Temperature')
                    BEGIN
                        ALTER TABLE [dbo].[charecterstics] ADD Temperature float NOT NULL DEFAULT 0.0;
                    END;

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Humidity')
                    BEGIN
                        ALTER TABLE [dbo].[charecterstics] ADD Humidity float NOT NULL DEFAULT 0.0;
                    END;

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Rainfall')
                    BEGIN
                        ALTER TABLE [dbo].[charecterstics] ADD Rainfall float NOT NULL DEFAULT 0.0;
                    END;

                    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[charecterstics]') AND name = 'Timestamp')
                    BEGIN
                        ALTER TABLE [dbo].[charecterstics] ADD Timestamp datetime2 NOT NULL DEFAULT GETUTCDATE();
                    END");
            }
            catch (Exception ex)
            {
                // Log the error but don't throw it
                Console.WriteLine($"Error ensuring columns exist: {ex.Message}");
            }
        }
    }
} 