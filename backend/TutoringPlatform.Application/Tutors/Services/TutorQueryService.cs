using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Application.Common.Exceptions;
using TutoringPlatform.Application.Common.Interfaces;
using TutoringPlatform.Application.Tutors.Dtos;

namespace TutoringPlatform.Application.Tutors.Services;

public class TutorQueryService
{
    private readonly IAppDbContext _db;

    public TutorQueryService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<TutorDetailsDto> GetDetailsAsync(Guid tutorProfileId)
    {
        var tutor = await _db.TutorProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == tutorProfileId);

        if (tutor is null)
            throw new NotFoundException("Tutor not found.");

        var offers = await (
            from o in _db.LessonOffers.AsNoTracking()
            join s in _db.Subjects.AsNoTracking() on o.SubjectId equals s.Id
            where o.TutorProfileId == tutorProfileId
            orderby s.Name, o.Price
            select new LessonOfferDto
            {
                Id = o.Id,
                SubjectId = o.SubjectId,
                SubjectName = s.Name,
                DurationMinutes = o.DurationMinutes,
                Price = o.Price,
                Mode = o.Mode
            }
        ).ToListAsync();

        return new TutorDetailsDto
        {
            Id = tutor.Id,
            UserId = tutor.UserId,
            DisplayName = tutor.DisplayName,
            Bio = tutor.Bio,
            City = tutor.City,
            HourlyRate = tutor.HourlyRate,
            Offers = offers
        };
    }

    public async Task<List<LessonOfferDto>> GetOffersAsync(Guid tutorProfileId)
    {
        var exists = await _db.TutorProfiles
            .AsNoTracking()
            .AnyAsync(x => x.Id == tutorProfileId);

        if (!exists)
            throw new NotFoundException("Tutor not found.");

        return await (
            from o in _db.LessonOffers.AsNoTracking()
            join s in _db.Subjects.AsNoTracking() on o.SubjectId equals s.Id
            where o.TutorProfileId == tutorProfileId
            orderby s.Name, o.Price
            select new LessonOfferDto
            {
                Id = o.Id,
                SubjectId = o.SubjectId,
                SubjectName = s.Name,
                DurationMinutes = o.DurationMinutes,
                Price = o.Price,
                Mode = o.Mode
            }
        ).ToListAsync();
    }
}
