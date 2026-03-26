using System.Text.Json.Serialization;

namespace TutoringPlatform.Mobile.Models;

public sealed class PagedResult<T>
{
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new();

    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }
}