using Microsoft.Maui.Storage;

namespace TutoringPlatform.Mobile.Storage;

public sealed class TokenStorage
{
    private const string Key = "auth_token";

    public Task SaveAsync(string token)
    {
        Preferences.Set(Key, token);
        return Task.CompletedTask;
    }

    public Task<string?> GetAsync()
    {
        var token = Preferences.Get(Key, null);
        return Task.FromResult(token);
    }

    public Task ClearAsync()
    {
        Preferences.Remove(Key);
        return Task.CompletedTask;
    }
}