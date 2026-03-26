namespace TutoringPlatform.Domain.Tutors;

public class TutorProfile
{
    public Guid Id { get; set; }

    // FK do AspNetUsers
    public Guid UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public string City { get; set; } = string.Empty;
}
