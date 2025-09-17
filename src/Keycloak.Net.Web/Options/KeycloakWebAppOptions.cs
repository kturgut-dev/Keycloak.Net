using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Keycloak.Net.Web.Options;

/// <summary>
///     Options for configuring MVC or Razor Pages integrations using OpenID Connect.
/// </summary>
public sealed class KeycloakWebAppOptions
{
    public string CookieScheme { get; set; } = CookieAuthenticationDefaults.AuthenticationScheme;

    public string OpenIdConnectScheme { get; set; } = OpenIdConnectDefaults.AuthenticationScheme;

    public bool RequireHttpsMetadata { get; set; } = true;

    public bool UsePkce { get; set; } = true;

    public Action<CookieAuthenticationOptions>? ConfigureCookie { get; set; }

    public Action<OpenIdConnectOptions>? ConfigureOpenIdConnect { get; set; }
}