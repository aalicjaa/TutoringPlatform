namespace TutoringPlatform.Domain.Offers;

public class LessonOffer
{
    public Guid Id { get; set; }

    public Guid TutorProfileId { get; set; }
    public int SubjectId { get; set; }

    public int DurationMinutes { get; set; } = 60;
    public decimal Price { get; set; }

    // np. "Online" / "OnSite"
    public string Mode { get; set; } = "Online";
}
