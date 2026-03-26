using System.Net;
using System.Net.Http.Json;
using TutoringPlatform.Mobile.Models;
using TutoringPlatform.Mobile.Storage;

namespace TutoringPlatform.Mobile.Api;

public sealed class AuthApi
{
    private readonly HttpClient _http;
    private readonly TokenStorage _tokenStorage;

    public AuthApi(HttpClient http, TokenStorage tokenStorage)
    {
        _http = http;
        _tokenStorage = tokenStorage;
    }

    public async Task<(bool Ok, string? Error)> LoginAsync(string email, string password, CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("api/Auth/login", new { email, password }, ct);

        if (resp.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            return (false, "Nieprawidłowy email lub hasło");

        if (!resp.IsSuccessStatusCode)
            return (false, $"Błąd logowania: {(int)resp.StatusCode}");

        var dto = await resp.Content.ReadFromJsonAsync<AuthResponseDto>(cancellationToken: ct);
        if (dto is null || string.IsNullOrWhiteSpace(dto.Token))
            return (false, "Brak tokenu w odpowiedzi");

        await _tokenStorage.SaveAsync(dto.Token);
        return (true, null);
    }

    public async Task<(bool Ok, string? Error)> RegisterAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        string role,
        CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("api/Auth/register", new { firstName, lastName, email, password, role }, ct);

        if (resp.StatusCode is HttpStatusCode.Conflict)
            return (false, "Konto o takim emailu już istnieje");

        if (resp.StatusCode is HttpStatusCode.BadRequest)
        {
            var msg = await TryReadErrorAsync(resp, ct) ?? "Nieprawidłowe dane rejestracji";
            return (false, msg);
        }

        if (!resp.IsSuccessStatusCode)
            return (false, $"Błąd rejestracji: {(int)resp.StatusCode}");

        return (true, null);
    }

    public async Task<(bool Ok, string? Error)> RegisterAndLoginAsync(
        string firstName,
        string lastName,
        string email,
        string password,
        string role,
        CancellationToken ct = default)
    {
        var reg = await RegisterAsync(firstName, lastName, email, password, role, ct);
        if (!reg.Ok) return reg;

        var login = await LoginAsync(email, password, ct);
        if (!login.Ok) return login;

        return (true, null);
    }

    public async Task LogoutAsync()
    {
        await _tokenStorage.ClearAsync();
    }

    public Task<string?> GetTokenAsync() => _tokenStorage.GetAsync();

    private static async Task<string?> TryReadErrorAsync(HttpResponseMessage resp, CancellationToken ct)
    {
        try
        {
            var text = await resp.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(text)) return null;
            return text.Trim().Trim('"');
        }
        catch
        {
            return null;
        }
    }
}