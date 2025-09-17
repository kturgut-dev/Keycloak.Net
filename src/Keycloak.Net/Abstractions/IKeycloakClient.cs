using Keycloak.Net.Models.Tokens;
using Keycloak.Net.Models.Users;

namespace Keycloak.Net.Abstractions;

/// <summary>
///     Defines a small set of high-level operations for interacting with Keycloak.
/// </summary>
public interface IKeycloakClient
{
    Task<KeycloakTokenResponse> RequestTokenAsync(KeycloakTokenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Retrieves an access token suitable for calling the Keycloak Admin REST API. The method automatically chooses the
    ///     appropriate grant type based on the configured credentials (client credentials vs. resource owner password).
    /// </summary>
    Task<KeycloakTokenResponse> GetManagementTokenAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<UserRepresentation>> GetUsersAsync(UserQueryParameters? query = null,
        CancellationToken cancellationToken = default);

    Task<UserRepresentation?> GetUserAsync(string userId, CancellationToken cancellationToken = default);

    Task UpdateUserEnabledStateAsync(string userId, bool enabled, CancellationToken cancellationToken = default);
}