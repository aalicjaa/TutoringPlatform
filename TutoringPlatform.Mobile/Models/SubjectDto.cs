using System.Text.Json.Serialization;

namespace TutoringPlatform.Mobile.Models;

public sealed class SubjectDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";
}