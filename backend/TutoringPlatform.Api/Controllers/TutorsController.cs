using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Domain.Tutors;
using TutoringPlatform.Infrastructure.Persistence;

namespace TutoringPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TutorsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TutorsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TutorProfile>>> Get()
    {
        var tutors = await _db.TutorProfiles.AsNoTracking().ToListAsync();
        return Ok(tutors);
    }
}
