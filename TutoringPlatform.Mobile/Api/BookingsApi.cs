using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TutoringPlatform.Mobile.Models;

namespace TutoringPlatform.Mobile.Api;

public sealed class BookingsApi
{
    private readonly HttpClient _http;

    public BookingsApi(HttpClient http)
    {
        _http = http;
    }

    private sealed record CreateBookingDto(
        [property: JsonPropertyName("lessonOfferId")] Guid LessonOfferId,
        [property: JsonPropertyName("startUtc")] DateTime StartUtc,
        [property: JsonPropertyName("endUtc")] DateTime EndUtc
    );

    private sealed record CreateBookingRequest([property: JsonPropertyName("dto")] CreateBookingDto Dto);

    public async Task CreateAsync(Guid lessonOfferId, DateTime startUtc, DateTime endUtc, CancellationToken ct = default)
    {
        var body = new CreateBookingRequest(new CreateBookingDto(lessonOfferId, startUtc, endUtc));
        using var resp = await _http.PostAsJsonAsync("api/Bookings", body, ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<List<MyBookingDto>> GetMineAsync(CancellationToken ct = default)
    {
        using var resp = await _http.GetAsync("api/Bookings/mine", ct);
        resp.EnsureSuccessStatusCode();

        var result = await resp.Content.ReadFromJsonAsync<List<MyBookingDto>>(cancellationToken: ct);
        return result ?? new List<MyBookingDto>();
    }

    public async Task<List<MyBookingDto>> GetMineAsTutorAsync(CancellationToken ct = default)
    {
        using var resp = await _http.GetAsync("api/Bookings/mine-as-tutor", ct);
        resp.EnsureSuccessStatusCode();

        var result = await resp.Content.ReadFromJsonAsync<List<MyBookingDto>>(cancellationToken: ct);
        return result ?? new List<MyBookingDto>();
    }

    public async Task CancelAsync(Guid bookingId, CancellationToken ct = default)
    {
        using var resp = await _http.PostAsync($"api/Bookings/{bookingId}/cancel", content: null, ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task CancelAsTutorAsync(Guid bookingId, CancellationToken ct = default)
    {
        using var resp = await _http.PostAsync($"api/Bookings/{bookingId}/cancel-as-tutor", content: null, ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task PayAsync(Guid bookingId, CancellationToken ct = default)
    {
        using var resp = await _http.PostAsync($"api/Bookings/{bookingId}/pay", content: null, ct);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<JsonDocument> GetMineJsonAsync(CancellationToken ct = default)
    {
        using var resp = await _http.GetAsync("api/Bookings/mine", ct);
        resp.EnsureSuccessStatusCode();

        var json = await resp.Content.ReadAsStringAsync(ct);
        return JsonDocument.Parse(json);
    }
}