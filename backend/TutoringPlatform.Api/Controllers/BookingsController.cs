using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TutoringPlatform.Application.Bookings.Commands;
using TutoringPlatform.Application.Bookings.Dtos;
using TutoringPlatform.Application.Bookings.Services;
using TutoringPlatform.Infrastructure.Persistence;

namespace TutoringPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly BookingService _service;
    private readonly AppDbContext _db;

    public BookingsController(BookingService service, AppDbContext db)
    {
        _service = service;
        _db = db;
    }

    public sealed class CreateBookingRequest
    {
        public CreateBookingDto Dto { get; set; } = default!;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBookingRequest request)
    {
        var studentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(studentUserIdStr, out var studentUserId))
            return Unauthorized(new { message = "Brak identyfikatora użytkownika w tokenie." });

        var id = await _service.CreateAsync(new CreateBookingCommand(request.Dto, studentUserId));
        return Ok(new { bookingId = id });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> Mine()
    {
        var studentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(studentUserIdStr, out var studentUserId))
            return Unauthorized(new { message = "Brak identyfikatora użytkownika w tokenie." });

        var items = await _service.GetMineAsync(studentUserId);
        return Ok(items);
    }

    [HttpGet("mine-as-tutor")]
    [Authorize(Roles = "Tutor")]
    public async Task<IActionResult> MineAsTutor()
    {
        var tutorUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(tutorUserIdStr, out var tutorUserId))
            return Unauthorized(new { message = "Brak identyfikatora użytkownika w tokenie." });

        var tutorProfileId = await _db.TutorProfiles
            .Where(t => t.UserId == tutorUserId)
            .Select(t => t.Id)
            .FirstOrDefaultAsync();

        if (tutorProfileId == Guid.Empty)
            return Ok(new List<MyBookingDto>());

        var items = await (
            from b in _db.Bookings.AsNoTracking()
            join o in _db.LessonOffers.AsNoTracking() on b.LessonOfferId equals o.Id
            where o.TutorProfileId == tutorProfileId
            orderby b.StartUtc descending
            select new MyBookingDto
            {
                Id = b.Id,
                LessonOfferId = b.LessonOfferId,
                StartUtc = b.StartUtc,
                EndUtc = b.EndUtc,
                Status = b.Status
            }
        ).ToListAsync();

        return Ok(items);
    }

    [HttpPost("{id:guid}/pay")]
    public async Task<IActionResult> Pay(Guid id)
    {
        var studentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(studentUserIdStr, out var studentUserId))
            return Unauthorized(new { message = "Brak identyfikatora użytkownika w tokenie." });

        await _service.PayAsync(id, studentUserId);
        return Ok(new { message = "Zajęcia zostały opłacone" });
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var studentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(studentUserIdStr, out var studentUserId))
            return Unauthorized(new { message = "Brak identyfikatora użytkownika w tokenie." });

        await _service.CancelAsync(id, studentUserId);
        return Ok(new { message = "Rezerwacja została anulowana" });
    }

    [HttpPost("{id:guid}/cancel-as-tutor")]
    [Authorize(Roles = "Tutor")]
    public async Task<IActionResult> CancelAsTutor(Guid id)
    {
        var tutorUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(tutorUserIdStr, out var tutorUserId))
            return Unauthorized(new { message = "Brak identyfikatora użytkownika w tokenie." });

        await _service.CancelAsTutorAsync(id, tutorUserId);
        return Ok(new { message = "Zajęcia zostały odwołane przez tutora" });
    }
}
