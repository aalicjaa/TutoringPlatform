namespace TutoringPlatform.Application.Bookings.Dtos;

public class MyBookingDto
{
    public Guid Id { get; set; }
    public Guid LessonOfferId { get; set; }
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }
    public string Status { get; set; } = "Pending";
}
