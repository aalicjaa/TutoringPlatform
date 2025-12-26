using Microsoft.AspNetCore.Mvc;
using TutoringPlatform.Domain.Tutors;

namespace TutoringPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TutorsController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<TutorProfile>> Get()
    {
        var tutors = new[]
        {
            new TutorProfile { DisplayName = "Anna Nowak", Bio = "Matematyka (liceum)", HourlyRate = 80, City = "Kraków" },
            new TutorProfile { DisplayName = "Jan Kowalski", Bio = "Angielski (B2/C1)", HourlyRate = 90, City = "Warszawa" },
        };

        return Ok(tutors);
    }
}
