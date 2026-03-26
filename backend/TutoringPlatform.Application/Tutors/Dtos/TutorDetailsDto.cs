namespace TutoringPlatform.Application.Tutors.Dtos;

public class TutorDetailsDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }

    public List<LessonOfferDto> Offers { get; set; } = new();
}
