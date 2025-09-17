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

    internal static string AdminBase(KeycloakOptions options)
    {
        return $"{EnsureTrailingSlash(options.BaseUrl)}admin/realms/{options.Realm}";
    }

    internal static string UserEndpoint(KeycloakOptions options, string userId)
    {
        return $"{AdminBase(options)}/users/{userId}";
    }

    internal static string UsersEndpoint(KeycloakOptions options, string? query = null)
    {
        var baseUrl = $"{AdminBase(options)}/users";
        return query is { Length: > 0 } ? baseUrl + query : baseUrl;
    }

    internal static string AttackDetectionUsers(KeycloakOptions options)
    {
        return $"{AdminBase(options)}/attack-detection/brute-force/users";
    }

    internal static string AttackDetectionUser(KeycloakOptions options, string userId)
    {
        return $"{AttackDetectionUsers(options)}/{userId}";
    }

    internal static string AuthenticationFlows(KeycloakOptions options)
    {
        return $"{AdminBase(options)}/authentication/flows";
    }

    internal static string AuthenticationFlow(KeycloakOptions options, string flowId)
    {
        return $"{AuthenticationFlows(options)}/{flowId}";
    }

    internal static string AuthenticationFlowExecutions(KeycloakOptions options, string flowAlias)
    {
        return $"{AuthenticationFlows(options)}/{flowAlias}/executions";
    }

    internal static string AuthenticationExecution(KeycloakOptions options, string executionId)
    {
        return $"{AdminBase(options)}/authentication/executions/{executionId}";
    }

    internal static string AuthenticationFlowCopy(KeycloakOptions options, string flowAlias)
    {
        return $"{AuthenticationFlows(options)}/{flowAlias}/copy";
    }
}