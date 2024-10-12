namespace ApogeeDev.IdentityProvider.Host.Models.Configuration;
public class OAuthWebProviderOptions
{
    public const string SectionName = nameof(OAuthWebProviderOptions);
    public List<OAuthWebProvider> Providers { get; set; } = [];

    public OAuthWebProvider Github =>
        Providers.FirstOrDefault(p => string.Compare(p.Name, "github", true) == 0)
        ?? throw new InvalidOperationException("Missing github config");

    public OAuthWebProvider Google =>
        Providers.FirstOrDefault(p => string.Compare(p.Name, "google", true) == 0)
        ?? throw new InvalidOperationException("Missing google config");

    public class OAuthWebProvider
    {
        public string Name { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string ClientSecret { get; set; } = default!;
        public string RedirectUri { get; set; } = default!;

        public string Scopes { get; set; } = default!;
    }
}
