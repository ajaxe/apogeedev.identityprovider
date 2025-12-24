using System.Diagnostics.CodeAnalysis;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ApogeeDev.IdentityProvider.Host.Models.Configuration;

public class AppClientOptions
{
    public const string SectionName = nameof(AppClientOptions);

    public List<AppClient> Clients { get; } = [];

    public void ThrowIfStaticClient(string clientId)
    {
        if (Clients.Any(c => c.ClientId == clientId))
        {
            throw new InvalidOperationException($"Static client Id '{clientId}' cannot be deleted");
        }
    }
}

public class AppClient
{
    public static readonly string OfflineAccessScope = Permissions.Prefixes.Scope + Scopes.OfflineAccess;
    public string ApplicationType { get; set; } = default!; // "native" or "web"
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string ClientType { get; set; } = default!; // "confidential" or "public"
    public string[] RedirectUris { get; set; } = [];
    public string[] PostLogoutRedirectUris { get; set; } = [];
    public string DisplayName { get; set; } = default!;

    public IEnumerable<Uri> GetRedirectUris() => RedirectUris.Select(uri => new Uri(uri));
    public IEnumerable<Uri> GetPostLogoutRedirectUris() => PostLogoutRedirectUris.Select(uri => new Uri(uri));

    public static IEnumerable<string> GetPermissionsWithOfflineAccess()
    {
        return [.. DefaultPermissions(), OfflineAccessScope, Permissions.GrantTypes.RefreshToken];
    }

    public static IEnumerable<string> DefaultPermissions()
    {
        yield return Permissions.Endpoints.Authorization;
        yield return Permissions.Endpoints.Token;
        yield return Permissions.Endpoints.EndSession;
        yield return Permissions.GrantTypes.AuthorizationCode;
        yield return Permissions.ResponseTypes.Code;
        yield return Permissions.Scopes.Address;
        yield return Permissions.Scopes.Email;
        yield return Permissions.Scopes.Phone;
        yield return Permissions.Scopes.Profile;
        yield return Permissions.Scopes.Roles;
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

