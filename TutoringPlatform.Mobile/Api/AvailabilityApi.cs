using System.Globalization;
using System.Text.Json;
using TutoringPlatform.Mobile.Models;

namespace TutoringPlatform.Mobile.Api;

public sealed class AvailabilityApi
{
    private readonly HttpClient _http;

    public AvailabilityApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<AvailabilitySlotDto>> GetFreeAsync(Guid tutorProfileId, DateTime localDate, CancellationToken ct = default)
    {
        var dateUtc = new DateTime(localDate.Year, localDate.Month, localDate.Day, 0, 0, 0, DateTimeKind.Utc);
        var dateParam = dateUtc.ToString("O", CultureInfo.InvariantCulture);

        var url = $"api/AvailabilitySlots/free?tutorProfileId={tutorProfileId}&dateUtc={Uri.EscapeDataString(dateParam)}";

        using var resp = await _http.GetAsync(url, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException($"{(int)resp.StatusCode} {resp.ReasonPhrase} | {url} | {body}");

        var result = JsonSerializer.Deserialize<List<AvailabilitySlotDto>>(
            body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return result ?? new List<AvailabilitySlotDto>();
    }
}