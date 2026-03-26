namespace TutoringPlatform.Application.Bookings.Commands;

public class CreateBookingResult
{
    public Guid BookingId { get; set; }
    public string Status { get; set; } = string.Empty;
}
