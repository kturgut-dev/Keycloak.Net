using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Keycloak.Net.Web.Options;

/// <summary>
///     Options for configuring API integrations that validate Keycloak access tokens.
/// </summary>
public sealed class KeycloakWebApiOptions
{
    public string AuthenticationScheme { get; set; } = JwtBearerDefaults.AuthenticationScheme;

    public bool RequireHttpsMetadata { get; set; } = true;

    public Action<JwtBearerOptions>? ConfigureJwtBearer { get; set; }
}