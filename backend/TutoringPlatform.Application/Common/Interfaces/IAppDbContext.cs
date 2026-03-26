
using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Domain.Availability;
using TutoringPlatform.Domain.Bookings;
using TutoringPlatform.Domain.Offers;
using TutoringPlatform.Domain.Payments;
using TutoringPlatform.Domain.Subjects;
using TutoringPlatform.Domain.Tutors;

namespace TutoringPlatform.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<TutorProfile> TutorProfiles { get; }
    DbSet<Subject> Subjects { get; }
    DbSet<LessonOffer> LessonOffers { get; }
    DbSet<AvailabilitySlot> AvailabilitySlots { get; }
    DbSet<Booking> Bookings { get; }
    DbSet<Payment> Payments { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
