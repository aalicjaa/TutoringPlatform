namespace TutoringPlatform.Application.Availability.Dtos;

public class CreateAvailabilitySlotDto
{
    public Guid TutorProfileId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
}
