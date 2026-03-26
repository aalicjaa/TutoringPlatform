using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Application.Common.Exceptions;
using TutoringPlatform.Infrastructure.Persistence;

namespace TutoringPlatform.Api.Controllers;

[ApiController]
[Route("api/my-tutor")]
[Authorize(Roles = "Tutor")]
public class MyTutorController : ControllerBase
{
    private readonly AppDbContext _db;

    public MyTutorController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("profile")]
    public async Task<ActionResult<MyTutorProfileDto>> GetMyProfile()
    {
        var tutor = await GetTutorProfileAsync();

        return Ok(ToProfileDto(tutor));
    }

    /// <summary>
    /// Tworzy profil tutora, jeżeli jeszcze nie istnieje.
    /// (Docelowo: tutor po pierwszym logowaniu uzupełnia dane i dopiero wtedy ma dostęp do ofert/dostępności.)
    /// </summary>
    [HttpPost("profile")]
    public async Task<ActionResult<MyTutorProfileDto>> CreateMyProfile(CreateMyTutorProfileDto dto)
    {
        var userId = GetUserId();

        var exists = await _db.TutorProfiles.AnyAsync(t => t.UserId == userId);
        if (exists)
            return Conflict(new { message = "Profil tutora już istnieje." });

        var displayName = (dto.DisplayName ?? "").Trim();
        var city = (dto.City ?? "").Trim();
        var bio = (dto.Bio ?? "").Trim();

        if (string.IsNullOrWhiteSpace(displayName))
            return BadRequest(new { message = "DisplayName jest wymagane." });

        if (string.IsNullOrWhiteSpace(city))
            return BadRequest(new { message = "City jest wymagane." });

        if (dto.HourlyRate < 0)
            return BadRequest(new { message = "HourlyRate nie może być ujemne." });

        var tutor = new TutoringPlatform.Domain.Tutors.TutorProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            DisplayName = displayName,
            City = city,
            Bio = bio,
            HourlyRate = dto.HourlyRate
        };

        _db.TutorProfiles.Add(tutor);
        await _db.SaveChangesAsync();

        return Ok(ToProfileDto(tutor));
    }

    [HttpPut("profile")]
    public async Task<ActionResult<MyTutorProfileDto>> UpdateMyProfile(UpdateMyTutorProfileDto dto)
    {
        var tutor = await GetTutorProfileAsync();

        var displayName = (dto.DisplayName ?? "").Trim();
        var city = (dto.City ?? "").Trim();
        var bio = (dto.Bio ?? "").Trim();

        if (string.IsNullOrWhiteSpace(displayName))
            return BadRequest(new { message = "DisplayName jest wymagane." });

        if (string.IsNullOrWhiteSpace(city))
            return BadRequest(new { message = "City jest wymagane." });

        if (dto.HourlyRate < 0)
            return BadRequest(new { message = "HourlyRate nie może być ujemne." });

        tutor.DisplayName = displayName;
        tutor.City = city;
        tutor.Bio = bio;
        tutor.HourlyRate = dto.HourlyRate;

        await _db.SaveChangesAsync();

        return Ok(ToProfileDto(tutor));
    }

    [HttpGet("offers")]
    public async Task<ActionResult<List<MyOfferDto>>> GetMyOffers()
    {
        var tutor = await GetTutorProfileAsync();

        var offers = await (
            from o in _db.LessonOffers.AsNoTracking()
            join s in _db.Subjects.AsNoTracking() on o.SubjectId equals s.Id
            where o.TutorProfileId == tutor.Id
            orderby s.Name, o.Price
            select new MyOfferDto
            {
                Id = o.Id,
                SubjectId = s.Id,
                SubjectName = s.Name,
                DurationMinutes = o.DurationMinutes,
                Price = o.Price,
                Mode = o.Mode
            }
        ).ToListAsync();

        return Ok(offers);
    }

    [HttpPost("offers")]
    public async Task<ActionResult<MyOfferDto>> CreateOffer(CreateMyOfferDto dto)
    {
        var tutor = await GetTutorProfileAsync();

        var subjectExists = await _db.Subjects.AnyAsync(s => s.Id == dto.SubjectId);
        if (!subjectExists)
            return BadRequest(new { message = "Nieprawidłowy subjectId." });

        if (dto.DurationMinutes <= 0)
            return BadRequest(new { message = "DurationMinutes musi być > 0." });

        if (dto.Price <= 0)
            return BadRequest(new { message = "Price musi być > 0." });

        var mode = (dto.Mode ?? "").Trim();
        if (mode != "Online" && mode != "Onsite")
            return BadRequest(new { message = "Mode musi mieć wartość Online albo Onsite." });

        var exists = await _db.LessonOffers.AnyAsync(o =>
            o.TutorProfileId == tutor.Id &&
            o.SubjectId == dto.SubjectId &&
            o.DurationMinutes == dto.DurationMinutes &&
            o.Price == dto.Price &&
            o.Mode == mode);

        if (exists)
            return Conflict(new { message = "Taka oferta już istnieje." });

        var offer = new TutoringPlatform.Domain.Offers.LessonOffer
        {
            Id = Guid.NewGuid(),
            TutorProfileId = tutor.Id,
            SubjectId = dto.SubjectId,
            DurationMinutes = dto.DurationMinutes,
            Price = dto.Price,
            Mode = mode
        };

        _db.LessonOffers.Add(offer);
        await _db.SaveChangesAsync();

        var subject = await _db.Subjects.AsNoTracking()
            .FirstAsync(s => s.Id == dto.SubjectId);

        return Ok(new MyOfferDto
        {
            Id = offer.Id,
            SubjectId = subject.Id,
            SubjectName = subject.Name,
            DurationMinutes = offer.DurationMinutes,
            Price = offer.Price,
            Mode = offer.Mode
        });
    }

    [HttpDelete("offers/{id:guid}")]
    public async Task<IActionResult> DeleteOffer(Guid id)
    {
        var tutor = await GetTutorProfileAsync();

        var offer = await _db.LessonOffers
            .FirstOrDefaultAsync(o => o.Id == id && o.TutorProfileId == tutor.Id);

        if (offer == null)
            return NotFound(new { message = "Nie znaleziono oferty." });

        _db.LessonOffers.Remove(offer);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet("availability")]
    public async Task<ActionResult<List<MyAvailabilityDto>>> GetAvailability(
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc)
    {
        var tutor = await GetTutorProfileAsync();

        if (toUtc <= fromUtc)
            return BadRequest(new { message = "Nieprawidłowy zakres dat." });

        var slots = await _db.AvailabilitySlots.AsNoTracking()
            .Where(x => x.TutorProfileId == tutor.Id && x.StartUtc < toUtc && x.EndUtc > fromUtc)
            .OrderBy(x => x.StartUtc)
            .Select(x => new MyAvailabilityDto
            {
                Id = x.Id,
                StartUtc = x.StartUtc,
                EndUtc = x.EndUtc
            })
            .ToListAsync();

        return Ok(slots);
    }

    [HttpPost("availability")]
    public async Task<ActionResult<MyAvailabilityDto>> CreateAvailability(CreateMyAvailabilityDto dto)
    {
        var tutor = await GetTutorProfileAsync();

        if (dto.EndUtc <= dto.StartUtc)
            return BadRequest(new { message = "EndUtc musi być > StartUtc." });

        var overlaps = await _db.AvailabilitySlots.AnyAsync(x =>
            x.TutorProfileId == tutor.Id &&
            x.StartUtc < dto.EndUtc &&
            x.EndUtc > dto.StartUtc);

        if (overlaps)
            return Conflict(new { message = "Dostępność nachodzi na istniejący termin." });

        var slot = new TutoringPlatform.Domain.Availability.AvailabilitySlot
        {
            Id = Guid.NewGuid(),
            TutorProfileId = tutor.Id,
            StartUtc = dto.StartUtc,
            EndUtc = dto.EndUtc
        };

        _db.AvailabilitySlots.Add(slot);
        await _db.SaveChangesAsync();

        return Ok(new MyAvailabilityDto
        {
            Id = slot.Id,
            StartUtc = slot.StartUtc,
            EndUtc = slot.EndUtc
        });
    }

    [HttpDelete("availability/{id:guid}")]
    public async Task<IActionResult> DeleteAvailability(Guid id)
    {
        var tutor = await GetTutorProfileAsync();

        var slot = await _db.AvailabilitySlots
            .FirstOrDefaultAsync(x => x.Id == id && x.TutorProfileId == tutor.Id);

        if (slot == null)
            return NotFound(new { message = "Nie znaleziono terminu dostępności." });

        _db.AvailabilitySlots.Remove(slot);
        await _db.SaveChangesAsync();

        return NoContent();
    }

    private Guid GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId))
            throw new UnauthorizedAccessException("Nieprawidłowy identyfikator użytkownika.");
        return userId;
    }

    private async Task<TutoringPlatform.Domain.Tutors.TutorProfile> GetTutorProfileAsync()
    {
        var userId = GetUserId();

        var tutor = await _db.TutorProfiles.FirstOrDefaultAsync(t => t.UserId == userId);
        if (tutor == null)
            throw new NotFoundException("Brak profilu tutora dla tego konta. Uzupełnij profil w panelu tutora.");

        return tutor;
    }

    private static MyTutorProfileDto ToProfileDto(TutoringPlatform.Domain.Tutors.TutorProfile tutor) => new()
    {
        TutorProfileId = tutor.Id,
        DisplayName = tutor.DisplayName,
        City = tutor.City,
        Bio = tutor.Bio,
        HourlyRate = tutor.HourlyRate
    };
}

/* DTOs */

public class MyTutorProfileDto
{
    public Guid TutorProfileId { get; set; }
    public string DisplayName { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Bio { get; set; } = default!;
    public decimal HourlyRate { get; set; }
}

public class CreateMyTutorProfileDto
{
    public string DisplayName { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Bio { get; set; } = default!;
    public decimal HourlyRate { get; set; }
}

public class UpdateMyTutorProfileDto
{
    public string DisplayName { get; set; } = default!;
    public string City { get; set; } = default!;
    public string Bio { get; set; } = default!;
    public decimal HourlyRate { get; set; }
}

public class MyOfferDto
{
    public Guid Id { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = default!;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public string Mode { get; set; } = default!;
}

public class CreateMyOfferDto
{
    public int SubjectId { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public string Mode { get; set; } = default!;
}

public class MyAvailabilityDto
{
    public Guid Id { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
}

public class CreateMyAvailabilityDto
{
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
}
