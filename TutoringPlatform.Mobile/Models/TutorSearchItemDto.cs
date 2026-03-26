using System.Text.Json.Serialization;

public sealed class TutorSearchItemDto
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("displayName")]
    public string? DisplayName { get; set; }

    [JsonPropertyName("bio")]
    public string? Bio { get; set; }

    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("minOfferPrice")]
    public decimal? MinOfferPrice { get; set; }

    [JsonPropertyName("subjects")]
    public List<SubjectDto> Subjects { get; set; } = new();
}

public sealed class SubjectDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }
}