namespace TutoringPlatform.Application.Tutors.Dtos;

public class LessonOfferDto
{
    public Guid Id { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }
    public string Mode { get; set; } = string.Empty;
}
