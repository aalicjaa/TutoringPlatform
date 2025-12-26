using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Domain.Tutors;

namespace TutoringPlatform.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TutorProfile> TutorProfiles => Set<TutorProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TutorProfile>(b =>
        {
            b.ToTable("TutorProfiles");
            b.HasKey(x => x.Id);
            b.Property(x => x.DisplayName).HasMaxLength(120).IsRequired();
            b.Property(x => x.Bio).HasMaxLength(1000);
            b.Property(x => x.City).HasMaxLength(80);
            b.Property(x => x.HourlyRate).HasColumnType("decimal(10,2)");
        });
    }
}
