using Microsoft.AspNetCore.Mvc;
using TutoringPlatform.Application.Tutors.Services;

namespace TutoringPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LessonOffersController : ControllerBase
{
    private readonly TutorQueryService _service;

    public LessonOffersController(TutorQueryService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid tutorProfileId)
    {
        var result = await _service.GetOffersAsync(tutorProfileId);
        return Ok(result);
    }
}
