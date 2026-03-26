namespace TutoringPlatform.Application.Bookings.Dtos;

public class CreateBookingDto
{
    public Guid LessonOfferId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
}
