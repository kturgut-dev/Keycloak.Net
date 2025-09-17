using Keycloak.Net.Configuration;

namespace Keycloak.Net.Models.Tokens;

/// <summary>
///     Describes the payload used to request a token from Keycloak.
/// </summary>
public sealed class KeycloakTokenRequest
{
    private KeycloakTokenRequest(string grantType)
    {
        GrantType = grantType;
    }

    /// <summary>
    ///     OAuth2 grant type (for example client_credentials or password).
    /// </summary>
    public string GrantType { get; }

    /// <summary>
    ///     Optional scope to request.
    /// </summary>
    public string? Scope { get; init; }

    /// <summary>
    ///     Username used in the resource owner password credentials grant.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    ///     Password used in the resource owner password credentials grant.
    /// </summary>
    public string? Password { get; init; }

    /// <summary>
    ///     Refresh token for refreshing an access token.
    /// </summary>
    public string? RefreshToken { get; init; }

    /// <summary>
    ///     Additional parameters passed to the token endpoint.
    /// </summary>
    public IDictionary<string, string> AdditionalParameters { get; } =
        new Dictionary<string, string>(StringComparer.Ordinal);

    /// <summary>
    ///     Creates a token request for the client credentials grant.
    /// </summary>
    public static KeycloakTokenRequest CreateClientCredentials(string? scope = null)
    {
        return new KeycloakTokenRequest("client_credentials")
        {
            Scope = scope
        };
    }

    /// <summary>
    ///     Creates a token request for the resource owner password credentials grant.
    /// </summary>
    public static KeycloakTokenRequest CreatePassword(string username, string password, string? scope = null)
    {
        return new KeycloakTokenRequest("password")
        {
            Username = username,
            Password = password,
            Scope = scope
        };
    }

    /// <summary>
    ///     Creates a token request for the refresh token grant.
    /// </summary>
    public static KeycloakTokenRequest CreateRefresh(string refreshToken)
    {
        return new KeycloakTokenRequest("refresh_token")
        {
            RefreshToken = refreshToken
        };
    }

    internal Dictionary<string, string> BuildPayload(KeycloakOptions options)
    {
        var payload = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["client_id"] = options.ClientId,
            ["grant_type"] = GrantType
        };

        if (!string.IsNullOrWhiteSpace(options.ClientSecret)) payload["client_secret"] = options.ClientSecret!;

        if (!string.IsNullOrWhiteSpace(Scope)) payload["scope"] = Scope!;

        if (!string.IsNullOrWhiteSpace(Username)) payload["username"] = Username!;

        if (!string.IsNullOrWhiteSpace(Password)) payload["password"] = Password!;

        if (!string.IsNullOrWhiteSpace(RefreshToken)) payload["refresh_token"] = RefreshToken!;

        foreach (var pair in AdditionalParameters) payload[pair.Key] = pair.Value;

        return payload;
    }
}