using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Data;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using ApogeeDev.IdentityProvider.Host.Models.DatabaseModels;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.Processors;

public class GoogleClaimsProcessor : ClaimsProcessorBase
{
    public GoogleClaimsProcessor(ApplicationDbContext dbContext,
        ILogger<GoogleClaimsProcessor> logger) : base(dbContext, logger)
    {
    }

    protected override Dictionary<string, string> IdClaimMap { get; } = new Dictionary<string, string>
    {
        [ClaimTypes.NameIdentifier] = Claims.Subject,
        [ClaimTypes.Name] = Claims.Name,
        [ClaimTypes.Email] = Claims.Email,
        [CustomClaimTypes.Common.IdentityProvider] = CustomClaimTypes.IdpServer.IdP,
        [Claims.Picture] = Claims.Picture,
        [Claims.Email] = Claims.Username,
    };

    protected override string GetExternalIdpName()
    {
        return Providers.Google;
    }
    protected override void ApplyClaims(AppUser user, ClaimsPrincipal principal)
    {
        ClaimsMapper.MapGoogleClaims(user, principal);
    }
}
