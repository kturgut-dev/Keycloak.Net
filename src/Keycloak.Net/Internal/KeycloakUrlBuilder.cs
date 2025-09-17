using Keycloak.Net.Configuration;

namespace Keycloak.Net.Internal;

internal static class KeycloakUrlBuilder
{
    internal static string EnsureTrailingSlash(string value)
    {
        return value.EndsWith('/') ? value : value + "/";
    }

    internal static string RealmBase(KeycloakOptions options)
    {
        return $"{EnsureTrailingSlash(options.BaseUrl)}realms/{options.Realm}";
    }

    internal static string OpenIdConnectBase(KeycloakOptions options)
    {
        return $"{RealmBase(options)}/protocol/openid-connect";
    }

    internal static string TokenEndpoint(KeycloakOptions options)
    {
        return $"{OpenIdConnectBase(options)}/token";
    }

    internal static string UserEndpoint(KeycloakOptions options, string userId)
    {
        return $"{EnsureTrailingSlash(options.BaseUrl)}admin/realms/{options.Realm}/users/{userId}";
    }

    internal static string UsersEndpoint(KeycloakOptions options, string? query = null)
    {
        var baseUrl = $"{EnsureTrailingSlash(options.BaseUrl)}admin/realms/{options.Realm}/users";
        return query is { Length: > 0 } ? baseUrl + query : baseUrl;
    }
}