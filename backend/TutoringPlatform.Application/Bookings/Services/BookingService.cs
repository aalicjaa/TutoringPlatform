using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Application.Bookings.Commands;
using TutoringPlatform.Application.Bookings.Dtos;
using TutoringPlatform.Application.Common.Exceptions;
using TutoringPlatform.Application.Common.Interfaces;
using TutoringPlatform.Domain.Bookings;
using TutoringPlatform.Domain.Payments;

namespace TutoringPlatform.Application.Bookings.Services;

public class BookingService
{
    private readonly IAppDbContext _db;

    public BookingService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> CreateAsync(CreateBookingCommand command)
    {
        if (command.Dto.StartUtc >= command.Dto.EndUtc)
            throw new ConflictException("Invalid time range.");

        var offer = await _db.LessonOffers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == command.Dto.LessonOfferId);

        if (offer == null)
            throw new NotFoundException("Lesson offer not found.");

        var overlap = await _db.Bookings.AnyAsync(b =>
            b.LessonOfferId == command.Dto.LessonOfferId &&
            b.StartUtc < command.Dto.EndUtc &&
            b.EndUtc > command.Dto.StartUtc &&
            !(
                (b.Status ?? "").ToLower() == "cancelled" ||
                (b.Status ?? "").ToLower() == "canceled" ||
                (b.Status ?? "").ToLower() == "cancelledbytutor"
            ));

        if (overlap)
            throw new ConflictException("Selected slot is no longer available.");

        var booking = new Booking
        {
            Id = Guid.NewGuid(),
            LessonOfferId = command.Dto.LessonOfferId,
            StudentUserId = command.StudentUserId,
            StartUtc = command.Dto.StartUtc,
            EndUtc = command.Dto.EndUtc,
            Status = "Pending"
        };

        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();

        return booking.Id;
    }

    public async Task<List<MyBookingDto>> GetMineAsync(Guid studentUserId)
    {
        return await _db.Bookings
            .AsNoTracking()
            .Where(b => b.StudentUserId == studentUserId)
            .OrderByDescending(b => b.StartUtc)
            .Select(b => new MyBookingDto
            {
                Id = b.Id,
                LessonOfferId = b.LessonOfferId,
                StartUtc = b.StartUtc,
                EndUtc = b.EndUtc,
                Status = b.Status
            })
            .ToListAsync();
    }

    public async Task PayAsync(Guid bookingId, Guid studentUserId)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b =>
            b.Id == bookingId && b.StudentUserId == studentUserId);

        if (booking == null)
            throw new NotFoundException("Booking not found.");

        if (string.Equals(booking.Status, "Paid", StringComparison.OrdinalIgnoreCase))
            return;

        var st = (booking.Status ?? "").Trim().ToLowerInvariant();
        if (st == "cancelled" || st == "canceled" || st == "cancelledbytutor")
            throw new ConflictException("Booking is cancelled.");

        var offer = await _db.LessonOffers
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == booking.LessonOfferId);

        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            BookingId = booking.Id,
            Provider = "Mock",
            Amount = offer?.Price ?? 0m,
            Currency = "PLN",
            Status = "Paid",
            CreatedUtc = DateTime.UtcNow
        };

        booking.Status = "Paid";

        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();
    }

    public async Task CancelAsync(Guid bookingId, Guid studentUserId)
    {
        var booking = await _db.Bookings.FirstOrDefaultAsync(b =>
            b.Id == bookingId && b.StudentUserId == studentUserId);

        if (booking == null)
            throw new NotFoundException("Booking not found.");

        if (string.Equals(booking.Status, "Paid", StringComparison.OrdinalIgnoreCase))
            throw new ConflictException("Cannot cancel a paid booking.");

        var st = (booking.Status ?? "").Trim().ToLowerInvariant();
        if (st == "cancelled" || st == "canceled" || st == "cancelledbytutor")
            return;

        booking.Status = "Cancelled";
        await _db.SaveChangesAsync();
    }

    public async Task CancelAsTutorAsync(Guid bookingId, Guid tutorUserId)
    {
        var tutorProfileId = await _db.TutorProfiles
            .Where(t => t.UserId == tutorUserId)
            .Select(t => t.Id)
            .FirstOrDefaultAsync();

        if (tutorProfileId == Guid.Empty)
            throw new NotFoundException("Tutor profile not found.");

        var booking = await _db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
        if (booking == null)
            throw new NotFoundException("Booking not found.");

        var belongs = await _db.LessonOffers.AnyAsync(o =>
            o.Id == booking.LessonOfferId && o.TutorProfileId == tutorProfileId);

        if (!belongs)
            throw new NotFoundException("Booking not found.");

        var st = (booking.Status ?? "").Trim().ToLowerInvariant();
        if (st == "cancelled" || st == "canceled" || st == "cancelledbytutor")
            return;

        booking.Status = "CancelledByTutor";
        await _db.SaveChangesAsync();
    }
}
