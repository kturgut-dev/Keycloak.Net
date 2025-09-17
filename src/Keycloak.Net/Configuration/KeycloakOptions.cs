using System.ComponentModel.DataAnnotations;

namespace Keycloak.Net.Configuration;

/// <summary>
///     Options controlling how the library connects to a Keycloak realm.
/// </summary>
public sealed class KeycloakOptions
{
    /// <summary>
    ///     Base URL of the Keycloak server, for example https://id.example.com/.
    /// </summary>
    [Required]
    [Url]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the realm that the application targets.
    /// </summary>
    [Required]
    public string Realm { get; set; } = string.Empty;

    /// <summary>
    ///     Client identifier used for token requests.
    /// </summary>
    [Required]
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    ///     Client secret used for confidential clients. Leave empty for public clients.
    /// </summary>
    public string? ClientSecret { get; set; }

    /// <summary>
    ///     Username for resource owner password credentials grant flow.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///     Password for resource owner password credentials grant flow.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    ///     Validates that the configuration is usable and throws an informative exception otherwise.
    /// </summary>
    public void Validate()
    {
        var validationContext = new ValidationContext(this);
        Validator.ValidateObject(this, validationContext, true);
    }
}