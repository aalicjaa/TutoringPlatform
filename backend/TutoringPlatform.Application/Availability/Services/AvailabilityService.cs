using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Application.Availability.Dtos;
using TutoringPlatform.Application.Common.Interfaces;

namespace TutoringPlatform.Application.Availability.Services;

public class AvailabilityService
{
    private readonly IAppDbContext _db;

    public AvailabilityService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<List<AvailabilitySlotDto>> GetAsync(Guid tutorProfileId, DateTime fromUtc, DateTime toUtc)
    {
        return await _db.AvailabilitySlots
            .Where(x => x.TutorProfileId == tutorProfileId &&
                        x.StartUtc < toUtc &&
                        x.EndUtc > fromUtc)
            .OrderBy(x => x.StartUtc)
            .Select(x => new AvailabilitySlotDto
            {
                Id = x.Id,
                TutorProfileId = x.TutorProfileId,
                StartUtc = x.StartUtc,
                EndUtc = x.EndUtc
            })
            .ToListAsync();
    }

    public async Task<List<FreeSlotDto>> GetFreeSlotsAsync(Guid tutorProfileId, DateTime dateUtc)
    {
        var dayStart = dateUtc.Date;
        var dayEnd = dayStart.AddDays(1);

        var availability = await _db.AvailabilitySlots
            .Where(x => x.TutorProfileId == tutorProfileId &&
                        x.StartUtc < dayEnd &&
                        x.EndUtc > dayStart)
            .OrderBy(x => x.StartUtc)
            .ToListAsync();

        var tutorOfferIds = await _db.LessonOffers
            .Where(o => o.TutorProfileId == tutorProfileId)
            .Select(o => o.Id)
            .ToListAsync();

        var bookings = await _db.Bookings
            .Where(b => tutorOfferIds.Contains(b.LessonOfferId) &&
                        b.StartUtc < dayEnd &&
                        b.EndUtc > dayStart)
            .OrderBy(b => b.StartUtc)
            .ToListAsync();

        var freeSlots = new List<FreeSlotDto>();

        foreach (var slot in availability)
        {
            var cursor = slot.StartUtc;

            foreach (var booking in bookings)
            {
                if (booking.StartUtc >= slot.EndUtc)
                    break;

                if (booking.EndUtc <= cursor)
                    continue;

                if (booking.StartUtc > cursor)
                {
                    freeSlots.Add(new FreeSlotDto
                    {
                        StartUtc = cursor,
                        EndUtc = booking.StartUtc
                    });
                }

                cursor = booking.EndUtc;
            }

            if (cursor < slot.EndUtc)
            {
                freeSlots.Add(new FreeSlotDto
                {
                    StartUtc = cursor,
                    EndUtc = slot.EndUtc
                });
            }
        }

        return freeSlots;
    }
}
