using System.Globalization;
using System.Text;

namespace Keycloak.Net.Models.Users;

/// <summary>
///     Parameters for querying Keycloak users.
/// </summary>
public sealed class UserQueryParameters
{
    public string? Search { get; init; }

    public int? First { get; init; }

    public int? Max { get; init; }

    public bool? Exact { get; init; }

    internal string ToQueryString()
    {
        var builder = new StringBuilder();

        void Append(string name, string value)
        {
            builder.Append(builder.Length == 0 ? '?' : '&');
            builder.Append(Uri.EscapeDataString(name));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(value));
        }

        if (!string.IsNullOrWhiteSpace(Search)) Append("search", Search!);

        if (First.HasValue) Append("first", First.Value.ToString(CultureInfo.InvariantCulture));

        if (Max.HasValue) Append("max", Max.Value.ToString(CultureInfo.InvariantCulture));

        if (Exact.HasValue) Append("exact", Exact.Value ? "true" : "false");

        return builder.ToString();
    }
}