namespace TutoringPlatform.Domain.Bookings;

public class Booking
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid LessonOfferId { get; set; }
    public Guid StudentUserId { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public string Status { get; set; } = "Pending";
}
