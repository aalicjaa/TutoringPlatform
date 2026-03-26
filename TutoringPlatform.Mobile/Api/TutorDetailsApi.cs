using System.Net.Http.Json;
using TutoringPlatform.Mobile.Models;

namespace TutoringPlatform.Mobile.Api;

public sealed class TutorDetailsApi
{
    private readonly HttpClient _http;

    public TutorDetailsApi(HttpClient http)
    {
        _http = http;
    }

    public async Task<TutorDetailsDto?> GetAsync(Guid tutorId, CancellationToken ct = default)
    {
        using var resp = await _http.GetAsync($"api/Tutors/{tutorId}", ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<TutorDetailsDto>(cancellationToken: ct);
    }
}