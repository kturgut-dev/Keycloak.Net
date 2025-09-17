using System.Text.Json.Serialization;

namespace Keycloak.Net.Models.Common;

/// <summary>
///     Represents the error payload returned by Keycloak on unsuccessful requests.
/// </summary>
public sealed record ErrorResponse
{
    [JsonPropertyName("error")] public string? Error { get; init; }

    [JsonPropertyName("error_description")]
    public string? ErrorDescription { get; init; }

    [JsonPropertyName("message")] public string? Message { get; init; }
}