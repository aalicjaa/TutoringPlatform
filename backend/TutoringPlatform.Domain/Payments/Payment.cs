namespace TutoringPlatform.Domain.Payments;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid BookingId { get; set; }

    public string Provider { get; set; } = "Stripe";
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "PLN";
    public string Status { get; set; } = "Initiated";

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
