using Keycloak.Net.Configuration;
using Keycloak.Net.DependencyInjection;
using Keycloak.Net.Web.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Keycloak.Net.Web.DependencyInjection;

/// <summary>
///     Extension methods that make it easier to integrate Keycloak in ASP.NET Core API and MVC projects.
/// </summary>
public static class KeycloakAuthenticationExtensions
{
    public static IServiceCollection AddKeycloakWebApi(this IServiceCollection services,
        Action<KeycloakOptions> configureKeycloak, Action<KeycloakWebApiOptions>? configureWebApi = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        if (configureKeycloak is null) throw new ArgumentNullException(nameof(configureKeycloak));

        services.AddKeycloakCore(configureKeycloak);
        ConfigureJwtBearer(services, configureWebApi);
        return services;
    }

    public static IServiceCollection AddKeycloakWebApi(this IServiceCollection services, IConfiguration configuration,
        Action<KeycloakWebApiOptions>? configureWebApi = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        services.AddKeycloakCore(configuration);
        ConfigureJwtBearer(services, configureWebApi);
        return services;
    }

    public static IServiceCollection AddKeycloakWebApp(this IServiceCollection services,
        Action<KeycloakOptions> configureKeycloak, Action<KeycloakWebAppOptions>? configureWebApp = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        if (configureKeycloak is null) throw new ArgumentNullException(nameof(configureKeycloak));

        services.AddKeycloakCore(configureKeycloak);
        ConfigureOpenIdConnect(services, configureWebApp);
        return services;
    }

    public static IServiceCollection AddKeycloakWebApp(this IServiceCollection services, IConfiguration configuration,
        Action<KeycloakWebAppOptions>? configureWebApp = null)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        services.AddKeycloakCore(configuration);
        ConfigureOpenIdConnect(services, configureWebApp);
        return services;
    }

    private static void ConfigureJwtBearer(IServiceCollection services, Action<KeycloakWebApiOptions>? configure)
    {
        var options = new KeycloakWebApiOptions();
        configure?.Invoke(options);

        var authenticationBuilder = services.AddAuthentication(authOptions =>
        {
            authOptions.DefaultScheme = options.AuthenticationScheme;
            authOptions.DefaultAuthenticateScheme = options.AuthenticationScheme;
            authOptions.DefaultChallengeScheme = options.AuthenticationScheme;
        });

        authenticationBuilder.AddJwtBearer(options.AuthenticationScheme, _ => { });

        services.AddOptions<JwtBearerOptions>(options.AuthenticationScheme)
            .Configure<IOptions<KeycloakOptions>>((jwtOptions, keycloakOptions) =>
            {
                var keycloak = keycloakOptions.Value;
                var authority = BuildRealmBaseAddress(keycloak);
                jwtOptions.Authority = authority;
                jwtOptions.Audience = keycloak.ClientId;
                jwtOptions.RequireHttpsMetadata = options.RequireHttpsMetadata;
            });

        if (options.ConfigureJwtBearer is not null)
            services.PostConfigure<JwtBearerOptions>(options.AuthenticationScheme, options.ConfigureJwtBearer);
    }

    private static void ConfigureOpenIdConnect(IServiceCollection services, Action<KeycloakWebAppOptions>? configure)
    {
        var options = new KeycloakWebAppOptions();
        configure?.Invoke(options);

        var authenticationBuilder = services.AddAuthentication(authOptions =>
        {
            authOptions.DefaultScheme = options.CookieScheme;
            authOptions.DefaultChallengeScheme = options.OpenIdConnectScheme;
        });

        authenticationBuilder.AddCookie(options.CookieScheme, cookieOptions =>
        {
            cookieOptions.SlidingExpiration = true;
            options.ConfigureCookie?.Invoke(cookieOptions);
        });

        authenticationBuilder.AddOpenIdConnect(options.OpenIdConnectScheme, _ => { });

        services.AddOptions<OpenIdConnectOptions>(options.OpenIdConnectScheme)
            .Configure<IOptions<KeycloakOptions>>((oidcOptions, keycloakOptions) =>
            {
                var keycloak = keycloakOptions.Value;
                var authority = BuildRealmBaseAddress(keycloak);
                var metadataAddress = $"{authority}/.well-known/openid-configuration";

                oidcOptions.Authority = authority;
                oidcOptions.MetadataAddress = metadataAddress;
                oidcOptions.ClientId = keycloak.ClientId;
                oidcOptions.ClientSecret = keycloak.ClientSecret;
                oidcOptions.RequireHttpsMetadata = options.RequireHttpsMetadata;
                oidcOptions.ResponseType = OpenIdConnectResponseType.Code;
                oidcOptions.UsePkce = options.UsePkce;
                oidcOptions.SaveTokens = true;
                oidcOptions.GetClaimsFromUserInfoEndpoint = true;

                options.ConfigureOpenIdConnect?.Invoke(oidcOptions);
            });
    }

    private static string BuildRealmBaseAddress(KeycloakOptions options)
    {
        var baseUrl = options.BaseUrl.TrimEnd('/');
        return $"{baseUrl}/realms/{options.Realm}";
    }
}