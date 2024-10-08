using System.Diagnostics.CodeAnalysis;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ApogeeDev.IdentityProvider.Host.Models.Configuration;

public class AppClientOptions
{
    public const string SectionName = nameof(AppClientOptions);

    public List<AppClient> Clients { get; } = new List<AppClient>();
}

public class AppClient
{
    public string ApplicationType { get; set; } = default!; // "native" or "web"
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string ClientType { get; set; } = default!; // "confidential" or "public"
    public string[] RedirectUris { get; set; } = [];
    public IEnumerable<Uri> GetUris() => RedirectUris.Select(uri => new Uri(uri));

    public IEnumerable<string> GetPermissions()
    {
        yield return Permissions.Endpoints.Authorization;
        yield return Permissions.Endpoints.Token;
        yield return Permissions.GrantTypes.AuthorizationCode;
        yield return Permissions.ResponseTypes.Code;
    }

    private IEnumerable<object> GetMembers()
    {
        yield return ApplicationType;
        yield return ClientId;
        yield return ClientSecret;
        yield return ClientType;

        foreach (var r in RedirectUris.OrderByDescending(s => s, StringComparer.OrdinalIgnoreCase))
            yield return r;
    }

    public class AppClientEqualityComparer : IEqualityComparer<AppClient>
    {
        public bool Equals(AppClient? x, AppClient? y)
        {
            if (x == null || y == null) return false;
            if (ReferenceEquals(y, x)) return true;
            return x.GetMembers().SequenceEqual(y.GetMembers());
        }

        public int GetHashCode([DisallowNull] AppClient obj)
        {
            return obj.GetMembers().Aggregate(default(int), HashCode.Combine);
        }
    }
}

