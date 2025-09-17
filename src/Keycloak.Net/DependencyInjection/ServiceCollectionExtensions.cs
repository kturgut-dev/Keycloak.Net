using System.ComponentModel.DataAnnotations;
using Keycloak.Net.Abstractions;
using Keycloak.Net.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Keycloak.Net.DependencyInjection;

/// <summary>
///     Dependency injection helpers for configuring Keycloak services.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddKeycloakCore(this IServiceCollection services,
        Action<KeycloakOptions> configure)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        if (configure is null) throw new ArgumentNullException(nameof(configure));

        ConfigureOptions(services, configure);
        services.AddHttpClient<IKeycloakClient, KeycloakClient>();

        return services;
    }

    public static IServiceCollection AddKeycloakCore(this IServiceCollection services, IConfiguration configuration)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        if (configuration is null) throw new ArgumentNullException(nameof(configuration));

        ConfigureOptions(services, options => configuration.Bind(options));
        services.AddHttpClient<IKeycloakClient, KeycloakClient>();

        return services;
    }

    private static void ConfigureOptions(IServiceCollection services, Action<KeycloakOptions> configure)
    {
        services.AddOptions<KeycloakOptions>()
            .Configure(configure);

        services.AddSingleton<IValidateOptions<KeycloakOptions>>(new KeycloakOptionsValidator());
    }

    private sealed class KeycloakOptionsValidator : IValidateOptions<KeycloakOptions>
    {
        public ValidateOptionsResult Validate(string? name, KeycloakOptions options)
        {
            try
            {
                options.Validate();
            }
            catch (ValidationException ex)
            {
                return ValidateOptionsResult.Fail(ex.Message);
            }

            if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _))
                return ValidateOptionsResult.Fail("Keycloak BaseUrl must be an absolute URI.");

            if (string.IsNullOrWhiteSpace(options.Realm))
                return ValidateOptionsResult.Fail("Keycloak realm must be provided.");

            return ValidateOptionsResult.Success;
        }
    }
}