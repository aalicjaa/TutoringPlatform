using System.Text.Json.Serialization;

namespace TutoringPlatform.Mobile.Models;

public sealed class TutorOfferDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("subjectId")]
    public int SubjectId { get; set; }

    [JsonPropertyName("subjectName")]
    public string SubjectName { get; set; } = "";

    [JsonPropertyName("durationMinutes")]
    public int DurationMinutes { get; set; }

    [JsonPropertyName("price")]
    public double Price { get; set; }

    [JsonPropertyName("mode")]
    public string Mode { get; set; } = "";
}