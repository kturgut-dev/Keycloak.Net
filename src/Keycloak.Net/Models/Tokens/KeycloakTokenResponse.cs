using System.Text.Json.Serialization;

namespace Keycloak.Net.Models.Tokens;

/// <summary>
///     Represents the JSON response returned from the Keycloak token endpoint.
/// </summary>
public sealed record KeycloakTokenResponse
{
    [JsonPropertyName("access_token")] public string AccessToken { get; init; } = string.Empty;

    [JsonPropertyName("expires_in")] public int ExpiresIn { get; init; }

    [JsonPropertyName("refresh_expires_in")]
    public int RefreshExpiresIn { get; init; }

    [JsonPropertyName("token_type")] public string TokenType { get; init; } = string.Empty;

    [JsonPropertyName("scope")] public string? Scope { get; init; }

    [JsonPropertyName("refresh_token")] public string? RefreshToken { get; init; }

    [JsonPropertyName("id_token")] public string? IdToken { get; init; }

    [JsonPropertyName("session_state")] public string? SessionState { get; init; }
}