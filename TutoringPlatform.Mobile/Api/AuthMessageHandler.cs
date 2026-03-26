using System.Net.Http.Headers;
using TutoringPlatform.Mobile.Storage;

namespace TutoringPlatform.Mobile.Api;

public sealed class AuthMessageHandler : DelegatingHandler
{
    private readonly TokenStorage _tokenStorage;

    public AuthMessageHandler(TokenStorage tokenStorage)
    {
        _tokenStorage = tokenStorage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStorage.GetAsync();
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}