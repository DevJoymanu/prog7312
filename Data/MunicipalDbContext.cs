using Microsoft.EntityFrameworkCore;
using prog7312_st10161149_part1.Models;

namespace prog7312_st10161149_part1.Data
{
    public class MunicipalDbContext : DbContext
    {
        public MunicipalDbContext(DbContextOptions<MunicipalDbContext> options) : base(options)
        {
        }

        public DbSet<IssueReport> IssueReports { get; set; }
        public DbSet<Event> Events { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // IssueReport config
            modelBuilder.Entity<IssueReport>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Location).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.ReporterName).HasMaxLength(100);
                entity.Property(e => e.ReporterEmail).HasMaxLength(255);
                entity.Property(e => e.PhotoPath).HasMaxLength(500);
            });

            // Event config
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
                entity.Property(e => e.EventDate).IsRequired();
                entity.Property(e => e.Location).HasMaxLength(200);
                entity.Property(e => e.OrganizerName).HasMaxLength(100);
                entity.Property(e => e.ContactEmail).HasMaxLength(255);
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.Property(e => e.ViewCount).HasDefaultValue(0);

            });
        }
    }
}
