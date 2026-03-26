using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutoringPlatform.Infrastructure.Persistence;
using TutoringPlatform.Api.Dtos;


namespace TutoringPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly AppDbContext _db;

    public SubjectsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<SubjectDto>>> Get()
    {
        var items = await _db.Subjects.AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new SubjectDto { Id = x.Id, Name = x.Name })
            .ToListAsync();

        return Ok(items);
    }
}

