using Microsoft.EntityFrameworkCore;

namespace FrontendApp.Models
{
    public class IcristatDbContext : DbContext
    {
        public IcristatDbContext(DbContextOptions<IcristatDbContext> options)
            : base(options)
        {
        }

        public DbSet<Characterization> Characterizations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Characterization>().ToTable("charecterstics");
        }
    }
}