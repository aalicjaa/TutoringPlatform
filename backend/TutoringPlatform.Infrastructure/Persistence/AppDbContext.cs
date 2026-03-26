using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Application.Common.Interfaces;
using TutoringPlatform.Domain.Availability;
using TutoringPlatform.Domain.Bookings;
using TutoringPlatform.Domain.Offers;
using TutoringPlatform.Domain.Payments;
using TutoringPlatform.Domain.Subjects;
using TutoringPlatform.Domain.Tutors;
using TutoringPlatform.Infrastructure.Identity;

namespace TutoringPlatform.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<TutorProfile> TutorProfiles => Set<TutorProfile>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<LessonOffer> LessonOffers => Set<LessonOffer>();
    public DbSet<AvailabilitySlot> AvailabilitySlots => Set<AvailabilitySlot>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TutorProfile ↔ AppUser (1:1 przez unikalny UserId)
        modelBuilder.Entity<TutorProfile>(b =>
        {
            b.ToTable("TutorProfiles");
            b.HasKey(x => x.Id);

            b.Property(x => x.DisplayName).HasMaxLength(120).IsRequired();
            b.Property(x => x.Bio).HasMaxLength(1000);
            b.Property(x => x.City).HasMaxLength(80);
            b.Property(x => x.HourlyRate).HasColumnType("decimal(10,2)").IsRequired();

            b.HasIndex(x => x.UserId).IsUnique();

            b.HasOne<AppUser>()
                .WithMany() // brak nawigacji po stronie AppUser
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Subject
        modelBuilder.Entity<Subject>(b =>
        {
            b.ToTable("Subjects");
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).HasMaxLength(100).IsRequired();
            b.HasIndex(x => x.Name).IsUnique();
        });

        // LessonOffer ↔ TutorProfile, Subject
        modelBuilder.Entity<LessonOffer>(b =>
        {
            b.ToTable("LessonOffers");
            b.HasKey(x => x.Id);

            b.Property(x => x.DurationMinutes).IsRequired();
            b.Property(x => x.Price).HasColumnType("decimal(10,2)").IsRequired();
            b.Property(x => x.Mode).HasMaxLength(20).IsRequired();

            b.HasOne<TutorProfile>()
                .WithMany()
                .HasForeignKey(x => x.TutorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<Subject>()
                .WithMany()
                .HasForeignKey(x => x.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.TutorProfileId, x.SubjectId });
        });

        // AvailabilitySlot ↔ TutorProfile
        modelBuilder.Entity<AvailabilitySlot>(b =>
        {
            b.ToTable("AvailabilitySlots");
            b.HasKey(x => x.Id);

            b.Property(x => x.StartUtc).IsRequired();
            b.Property(x => x.EndUtc).IsRequired();

            b.HasOne<TutorProfile>()
                .WithMany()
                .HasForeignKey(x => x.TutorProfileId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.TutorProfileId, x.StartUtc, x.EndUtc });
        });

        // Booking ↔ LessonOffer, Student(AppUser)
        modelBuilder.Entity<Booking>(b =>
        {
            b.ToTable("Bookings");
            b.HasKey(x => x.Id);

            b.Property(x => x.StartUtc).IsRequired();
            b.Property(x => x.EndUtc).IsRequired();
            b.Property(x => x.Status).HasMaxLength(30).IsRequired();

            b.HasOne<LessonOffer>()
                .WithMany()
                .HasForeignKey(x => x.LessonOfferId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.StudentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.LessonOfferId, x.StartUtc, x.EndUtc });
            b.HasIndex(x => new { x.StudentUserId, x.StartUtc });
        });

        // Payment ↔ Booking (1:1)
        modelBuilder.Entity<Payment>(b =>
        {
            b.ToTable("Payments");
            b.HasKey(x => x.Id);

            b.Property(x => x.Provider).HasMaxLength(30).IsRequired();
            b.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            b.Property(x => x.Status).HasMaxLength(30).IsRequired();
            b.Property(x => x.Amount).HasColumnType("decimal(10,2)").IsRequired();

            b.HasIndex(x => x.BookingId).IsUnique();

            b.HasOne<Booking>()
                .WithMany()
                .HasForeignKey(x => x.BookingId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
