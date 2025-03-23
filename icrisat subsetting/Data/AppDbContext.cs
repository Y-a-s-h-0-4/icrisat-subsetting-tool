using Microsoft.EntityFrameworkCore;
using IcrisatSubsetting.Models;

namespace IcrisatSubsetting.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<PassportData> PassportData { get; set; }
        public DbSet<Characteristics> Characteristics { get; set; }
    }
}
