using System.Text.Json.Serialization;

namespace TutoringPlatform.Mobile.Models;

public sealed class TutorDetailsDto
{
    [JsonPropertyName("id")]
    public Guid TutorProfileId { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("bio")]
    public string? Bio { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("offers")]
    public List<TutorOfferDto> Offers { get; set; } = new();
}