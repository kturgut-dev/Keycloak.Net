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

    Task<IReadOnlyList<UserRepresentation>> GetUsersAsync(UserQueryParameters? query = null,
        CancellationToken cancellationToken = default);

    Task<UserRepresentation?> GetUserAsync(string userId, CancellationToken cancellationToken = default);

    Task UpdateUserEnabledStateAsync(string userId, bool enabled, CancellationToken cancellationToken = default);
}