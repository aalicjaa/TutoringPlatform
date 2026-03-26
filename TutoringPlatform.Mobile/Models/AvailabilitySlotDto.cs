using System.Text.Json.Serialization;

namespace TutoringPlatform.Mobile.Models;

public sealed class AvailabilitySlotDto
{
    [JsonPropertyName("startUtc")]
    public DateTime StartUtc { get; set; }

    [JsonPropertyName("endUtc")]
    public DateTime EndUtc { get; set; }

    [JsonIgnore]
    public bool IsSelected { get; set; }
}