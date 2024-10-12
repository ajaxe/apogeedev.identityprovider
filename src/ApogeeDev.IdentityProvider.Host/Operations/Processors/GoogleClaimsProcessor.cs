using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Data;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.Processors;

public class GoogleClaimsProcessor : ClaimsProcessorBase
{
    public GoogleClaimsProcessor(ApplicationDbContext dbContext, ILogger logger)
        : base(dbContext, logger)
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
}
