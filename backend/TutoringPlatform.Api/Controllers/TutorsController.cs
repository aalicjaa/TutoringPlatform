using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Infrastructure.Persistence;

namespace TutoringPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TutorsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TutorsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("search")]
    public async Task<ActionResult<PagedResult<TutorSearchItemDto>>> Search(
        [FromQuery] int? subjectId,
        [FromQuery] string? subjectName,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? city,
        [FromQuery] string? mode,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize;
        pageSize = pageSize > 100 ? 100 : pageSize;

        IQueryable<Guid>? filteredTutorIds = null;

        var offers = _db.LessonOffers.AsNoTracking();

        if (subjectId.HasValue)
            offers = offers.Where(o => o.SubjectId == subjectId.Value);

        if (!string.IsNullOrWhiteSpace(subjectName))
        {
            var sn = subjectName.Trim().ToLowerInvariant();
            offers =
                from o in offers
                join s in _db.Subjects.AsNoTracking() on o.SubjectId equals s.Id
                where s.Name.ToLower() == sn
                select o;
        }

        if (!string.IsNullOrWhiteSpace(mode))
        {
            var m = mode.Trim();
            offers = offers.Where(o => o.Mode == m);
        }

        if (minPrice.HasValue)
            offers = offers.Where(o => o.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            offers = offers.Where(o => o.Price <= maxPrice.Value);

        var offerFiltersUsed =
            subjectId.HasValue ||
            !string.IsNullOrWhiteSpace(subjectName) ||
            !string.IsNullOrWhiteSpace(mode) ||
            minPrice.HasValue ||
            maxPrice.HasValue;

        if (offerFiltersUsed)
            filteredTutorIds = offers.Select(o => o.TutorProfileId).Distinct();

        var tutorsQuery = _db.TutorProfiles.AsNoTracking().AsQueryable();

        if (filteredTutorIds != null)
            tutorsQuery = tutorsQuery.Where(t => filteredTutorIds.Contains(t.Id));

        if (!string.IsNullOrWhiteSpace(city))
        {
            var c = city.Trim().ToLowerInvariant();
            tutorsQuery = tutorsQuery.Where(t => t.City.ToLower().Contains(c));
        }

        var total = await tutorsQuery.CountAsync();

        var tutors = await tutorsQuery
            .OrderBy(t => t.DisplayName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TutorSearchBaseDto
            {
                Id = t.Id,
                DisplayName = t.DisplayName,
                Bio = t.Bio,
                City = t.City,
                HourlyRate = t.HourlyRate
            })
            .ToListAsync();

        var tutorIds = tutors.Select(t => t.Id).ToList();

        var offerRows = await (
            from o in _db.LessonOffers.AsNoTracking()
            join s in _db.Subjects.AsNoTracking() on o.SubjectId equals s.Id
            where tutorIds.Contains(o.TutorProfileId)
            select new OfferRowDto
            {
                TutorProfileId = o.TutorProfileId,
                OfferId = o.Id,
                SubjectId = s.Id,
                SubjectName = s.Name,
                DurationMinutes = o.DurationMinutes,
                Price = o.Price,
                Mode = o.Mode
            }
        ).ToListAsync();

        var offersByTutor = offerRows
            .GroupBy(x => x.TutorProfileId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var items = tutors.Select(t =>
        {
            offersByTutor.TryGetValue(t.Id, out var list);
            list ??= new List<OfferRowDto>();

            return new TutorSearchItemDto
            {
                Id = t.Id,
                DisplayName = t.DisplayName,
                Bio = t.Bio,
                City = t.City,
                HourlyRate = t.HourlyRate,
                MinOfferPrice = list.Count == 0 ? null : list.Min(x => x.Price),
                Subjects = list
                    .GroupBy(x => new { x.SubjectId, x.SubjectName })
                    .Select(g => new SubjectDto
                    {
                        Id = g.Key.SubjectId,
                        Name = g.Key.SubjectName
                    })
                    .OrderBy(x => x.Name)
                    .ToList(),
                Offers = list
                    .OrderBy(x => x.SubjectName)
                    .ThenBy(x => x.Price)
                    .Select(x => new OfferSummaryDto
                    {
                        Id = x.OfferId,
                        SubjectId = x.SubjectId,
                        SubjectName = x.SubjectName,
                        DurationMinutes = x.DurationMinutes,
                        Price = x.Price,
                        Mode = x.Mode
                    })
                    .ToList()
            };
        }).ToList();

        return Ok(new PagedResult<TutorSearchItemDto>
        {
            Items = items,
            Total = total,
            Page = page,
            PageSize = pageSize
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TutorDetailsDto>> GetById(Guid id)
    {
        var tutor = await _db.TutorProfiles.AsNoTracking()
            .Where(t => t.Id == id)
            .Select(t => new TutorDetailsDto
            {
                Id = t.Id,
                DisplayName = t.DisplayName,
                Bio = t.Bio,
                City = t.City,
                HourlyRate = t.HourlyRate,
                Subjects = new List<SubjectDto>(),
                Offers = new List<OfferSummaryDto>()
            })
            .FirstOrDefaultAsync();

        if (tutor == null)
            return NotFound(new { message = "Tutor not found." });

        var offerRows = await (
            from o in _db.LessonOffers.AsNoTracking()
            join s in _db.Subjects.AsNoTracking() on o.SubjectId equals s.Id
            where o.TutorProfileId == id
            select new OfferSummaryDto
            {
                Id = o.Id,
                SubjectId = s.Id,
                SubjectName = s.Name,
                DurationMinutes = o.DurationMinutes,
                Price = o.Price,
                Mode = o.Mode
            }
        ).OrderBy(x => x.SubjectName).ThenBy(x => x.Price).ToListAsync();

        tutor.Offers = offerRows;

        tutor.Subjects = offerRows
            .GroupBy(x => new { x.SubjectId, x.SubjectName })
            .Select(g => new SubjectDto { Id = g.Key.SubjectId, Name = g.Key.SubjectName })
            .OrderBy(x => x.Name)
            .ToList();

        tutor.MinOfferPrice = offerRows.Count == 0 ? null : offerRows.Min(x => x.Price);

        return Ok(tutor);
    }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class TutorSearchBaseDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
}

public class TutorSearchItemDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public decimal? MinOfferPrice { get; set; }
    public List<SubjectDto> Subjects { get; set; } = new();
    public List<OfferSummaryDto> Offers { get; set; } = new();
}

public class TutorDetailsDto
{
    public Guid Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public decimal? MinOfferPrice { get; set; }
    public List<SubjectDto> Subjects { get; set; } = new();
    public List<OfferSummaryDto> Offers { get; set; } = new();
}

public class OfferSummaryDto
{
    public Guid Id { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public string Mode { get; set; } = string.Empty;
}

public class OfferRowDto
{
    public Guid TutorProfileId { get; set; }
    public Guid OfferId { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public string Mode { get; set; } = string.Empty;
}

public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
