using System.Net;
using Keycloak.Net.Models.Common;

namespace Keycloak.Net.Exceptions;

/// <summary>
///     Exception thrown when Keycloak returns an unsuccessful HTTP status code.
/// </summary>
public sealed class KeycloakHttpException : Exception
{
    public KeycloakHttpException(HttpStatusCode statusCode, ErrorResponse? error, string? content = null)
        : base(BuildMessage(statusCode, error, content))
    {
        StatusCode = statusCode;
        Error = error;
        Content = content;
    }

    public HttpStatusCode StatusCode { get; }

    public ErrorResponse? Error { get; }

    public string? Content { get; }

    private static string BuildMessage(HttpStatusCode statusCode, ErrorResponse? error, string? content)
    {
        if (error is null) return $"Keycloak request failed with status code {(int)statusCode} ({statusCode}).";

        var description = error.ErrorDescription ?? error.Message;
        if (!string.IsNullOrEmpty(description))
            return $"Keycloak request failed with status code {(int)statusCode} ({statusCode}): {description}";

        if (!string.IsNullOrEmpty(error.Error))
            return $"Keycloak request failed with status code {(int)statusCode} ({statusCode}): {error.Error}";

        return $"Keycloak request failed with status code {(int)statusCode} ({statusCode}).";
    }
}