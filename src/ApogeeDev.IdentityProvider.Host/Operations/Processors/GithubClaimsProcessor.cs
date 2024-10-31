using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Data;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using ApogeeDev.IdentityProvider.Host.Models.DatabaseModels;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.Processors;

public class GithubClaimsProcessor : ClaimsProcessorBase
{
    protected override Dictionary<string, string> IdClaimMap { get; } = new Dictionary<string, string>
    {
        [ClaimTypes.NameIdentifier] = Claims.Subject,
        [ClaimTypes.Name] = Claims.Name,
        [ClaimTypes.Email] = Claims.Email,
        [CustomClaimTypes.Common.IdentityProvider] = CustomClaimTypes.IdpServer.IdP,
        [CustomClaimTypes.GitHub.AvatarUrl] = Claims.Picture,
        [CustomClaimTypes.GitHub.Login] = Claims.Username,
    };

    public GithubClaimsProcessor(ApplicationDbContext dbContext,
        ILogger<GithubClaimsProcessor> logger) : base(dbContext, logger) { }

    protected override string GetExternalIdpName()
    {
        return Providers.GitHub;
    }
    protected override void ApplyClaims(AppUser user, ClaimsPrincipal principal)
    {
        ClaimsMapper.MapGithubClaims(user, principal);
    }
}