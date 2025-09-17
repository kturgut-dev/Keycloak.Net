using System.Text.Json.Serialization;

namespace Keycloak.Net.Models.Users;

/// <summary>
///     Minimal representation of a Keycloak user returned by the Admin REST API.
/// </summary>
public sealed record UserRepresentation
{
    [JsonPropertyName("id")] public string Id { get; init; } = string.Empty;

    [JsonPropertyName("username")] public string? Username { get; init; }

    [JsonPropertyName("email")] public string? Email { get; init; }

    [JsonPropertyName("firstName")] public string? FirstName { get; init; }

    [JsonPropertyName("lastName")] public string? LastName { get; init; }

    [JsonPropertyName("enabled")] public bool Enabled { get; init; }

    [JsonPropertyName("emailVerified")] public bool EmailVerified { get; init; }

    [JsonPropertyName("attributes")] public IReadOnlyDictionary<string, object>? Attributes { get; init; }
}