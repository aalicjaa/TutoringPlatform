namespace TutoringPlatform.Application.Availability.Dtos;

public class AvailabilitySlotDto
{
    public Guid Id { get; set; }
    public Guid TutorProfileId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
}
