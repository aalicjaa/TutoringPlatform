using System.Text.Json.Serialization;

namespace TutoringPlatform.Mobile.Models;

public sealed class MyBookingDto
{
    // mapowanie z API: "id"
    [JsonPropertyName("id")]
    public Guid BookingId { get; set; }

    // mapowanie z API: "startUtc" / "endUtc"
    [JsonPropertyName("startUtc")]
    public DateTime StartUtc { get; set; }

    [JsonPropertyName("endUtc")]
    public DateTime EndUtc { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; } = "";

    // Pola do UI (nie z JSON)
    [JsonIgnore]
    public DateOnly Date => DateOnly.FromDateTime(StartUtc.ToLocalTime());

    [JsonIgnore]
    public TimeOnly StartTime => TimeOnly.FromDateTime(StartUtc.ToLocalTime());

    [JsonIgnore]
    public TimeOnly EndTime => TimeOnly.FromDateTime(EndUtc.ToLocalTime());
}