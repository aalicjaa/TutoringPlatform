using System.Text.Json.Serialization;

namespace TutoringPlatform.Mobile.Models;

public sealed class AuthResponseDto
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = "";

    [JsonPropertyName("email")]
    public string Email { get; set; } = "";

    [JsonPropertyName("role")]
    public string Role { get; set; } = "";
}