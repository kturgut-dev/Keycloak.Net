using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Keycloak.Net.Abstractions;
using Keycloak.Net.Configuration;
using Keycloak.Net.Exceptions;
using Keycloak.Net.Internal;
using Keycloak.Net.Models.Common;
using Keycloak.Net.Models.Tokens;
using Keycloak.Net.Models.Users;
using Microsoft.Extensions.Options;

namespace Keycloak.Net;

/// <summary>
///     Default HTTP-based implementation of <see cref="IKeycloakClient" />.
/// </summary>
public sealed class KeycloakClient : IKeycloakClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _options;

    public KeycloakClient(HttpClient httpClient, IOptions<KeycloakOptions> optionsAccessor)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
        _options.Validate();

        _httpClient.BaseAddress = new Uri(KeycloakUrlBuilder.EnsureTrailingSlash(_options.BaseUrl));
    }

    public async Task<KeycloakTokenResponse> RequestTokenAsync(KeycloakTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var payload = request.BuildPayload(_options);
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, KeycloakUrlBuilder.TokenEndpoint(_options))
        {
            Content = new FormUrlEncodedContent(payload)
        };

        using var response = await _httpClient
            .SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        return await HandleResponseAsync<KeycloakTokenResponse>(response, cancellationToken).ConfigureAwait(false)
               ?? throw new InvalidOperationException("Keycloak token response was empty.");
    }

    public async Task<IReadOnlyList<UserRepresentation>> GetUsersAsync(UserQueryParameters? query = null,
        CancellationToken cancellationToken = default)
    {
        var accessToken = await GetManagementTokenAsync(cancellationToken).ConfigureAwait(false);
        var endpoint = KeycloakUrlBuilder.UsersEndpoint(_options, query?.ToQueryString());

        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

        using var response = await _httpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        var users = await HandleResponseAsync<List<UserRepresentation>>(response, cancellationToken)
            .ConfigureAwait(false);
        return (IReadOnlyList<UserRepresentation>?)users ?? Array.Empty<UserRepresentation>();
    }

    public async Task<UserRepresentation?> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userId));

        var accessToken = await GetManagementTokenAsync(cancellationToken).ConfigureAwait(false);
        var endpoint = KeycloakUrlBuilder.UserEndpoint(_options, userId);

        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

        using var response = await _httpClient
            .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        return await HandleResponseAsync<UserRepresentation>(response, cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateUserEnabledStateAsync(string userId, bool enabled,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(userId));

        var accessToken = await GetManagementTokenAsync(cancellationToken).ConfigureAwait(false);
        var endpoint = KeycloakUrlBuilder.UserEndpoint(_options, userId);

        using var request = new HttpRequestMessage(HttpMethod.Put, endpoint)
        {
            Content = JsonContent.Create(new { enabled })
        };
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.AccessToken);

        using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessStatusCode) await ThrowForErrorAsync(response, cancellationToken).ConfigureAwait(false);
    }

    private async Task<KeycloakTokenResponse> GetManagementTokenAsync(CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_options.Username) && !string.IsNullOrWhiteSpace(_options.Password))
        {
            var passwordRequest = KeycloakTokenRequest.CreatePassword(_options.Username!, _options.Password!);
            return await RequestTokenAsync(passwordRequest, cancellationToken).ConfigureAwait(false);
        }

        return await RequestTokenAsync(KeycloakTokenRequest.CreateClientCredentials(), cancellationToken)
            .ConfigureAwait(false);
    }

    private static async Task<T?> HandleResponseAsync<T>(HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            if (response.Content.Headers.ContentLength == 0) return default;

            await using var contentStream =
                await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
            if (contentStream is null) return default;

            return await JsonSerializer.DeserializeAsync<T>(contentStream, SerializerOptions, cancellationToken)
                .ConfigureAwait(false);
        }

        await ThrowForErrorAsync(response, cancellationToken).ConfigureAwait(false);
        return default;
    }

    private static async Task ThrowForErrorAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        ErrorResponse? error = null;
        string? content = null;

        if (response.Content is not null)
        {
            content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(content))
                try
                {
                    error = JsonSerializer.Deserialize<ErrorResponse>(content, SerializerOptions);
                }
                catch (JsonException)
                {
                    // Ignore JSON parse errors and rely on the raw content.
                }
        }

        throw new KeycloakHttpException(response.StatusCode, error, content);
    }
}