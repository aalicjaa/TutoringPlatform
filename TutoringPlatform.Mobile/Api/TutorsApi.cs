using System.Net.Http.Json;
using TutoringPlatform.Mobile.Models;

namespace TutoringPlatform.Mobile.Api;

public sealed class TutorsApi
{
    private readonly HttpClient _http;

    public TutorsApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<TutorSearchItemDto>> GetAsync(CancellationToken ct = default)
    {
        using var response = await _http.GetAsync("api/tutors/search", ct);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PagedResult<TutorSearchItemDto>>(cancellationToken: ct);
        return result?.Items ?? new List<TutorSearchItemDto>();
    }
}