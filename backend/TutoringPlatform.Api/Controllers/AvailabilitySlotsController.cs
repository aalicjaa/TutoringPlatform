using Microsoft.AspNetCore.Mvc;
using TutoringPlatform.Application.Availability.Services;

namespace TutoringPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilitySlotsController : ControllerBase
{
    private readonly AvailabilityService _service;

    public AvailabilitySlotsController(AvailabilityService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] Guid tutorProfileId,
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc)
    {
        if (tutorProfileId == Guid.Empty)
            return BadRequest("tutorProfileId is required.");

        if (fromUtc >= toUtc)
            return BadRequest("Invalid time range.");

        var result = await _service.GetAsync(tutorProfileId, fromUtc, toUtc);
        return Ok(result);
    }

    [HttpGet("free")]
    public async Task<IActionResult> GetFree(
        [FromQuery] Guid tutorProfileId,
        [FromQuery] DateTime dateUtc)
    {
        if (tutorProfileId == Guid.Empty)
            return BadRequest("tutorProfileId is required.");

        var result = await _service.GetFreeSlotsAsync(tutorProfileId, dateUtc);
        return Ok(result);
    }
}
