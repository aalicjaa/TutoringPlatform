namespace TutoringPlatform.Mobile.Models;

public sealed class CreateBookingRequest
{
    public Guid TutorId { get; set; }
    public DateTime StartsAt { get; set; }
}