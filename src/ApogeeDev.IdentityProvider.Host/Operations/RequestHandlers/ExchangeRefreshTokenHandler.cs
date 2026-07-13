using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Data;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using OpenIddict.Abstractions;
using Microsoft.EntityFrameworkCore;

using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class ExchangeRefreshTokenRequest : IRequest<ClaimsPrincipal?>
{
    public ClaimsPrincipal OriginalPrincipal { get; set; } = null!;
    public string[] RequestedScopes { get; set; } = Array.Empty<string>();
}

public class ExchangeRefreshTokenRequestHandler(ApplicationDbContext dbContext, ILogger<ExchangeRefreshTokenRequestHandler> logger)
    : IRequestHandler<ExchangeRefreshTokenRequest, ClaimsPrincipal?>
{
    public async Task<ClaimsPrincipal?> Handle(ExchangeRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var subject = request.OriginalPrincipal.GetClaim(Claims.Subject);
        var idp = request.OriginalPrincipal.GetClaim(CustomClaimTypes.IdpServer.IdP);

        // 1. Verify user still exists
        var user = await dbContext.AppUsers
            .FirstOrDefaultAsync(u => u.Subject == subject && u.IdentityProvider == idp, cancellationToken);

        if (user == null)
        {
            logger.LogWarning("User with subject {Subject} and IdP {IdP} not found during refresh token exchange.", subject, idp);
            return null;
        }

        // 2. Clone the principal
        var claimsPrincipal = request.OriginalPrincipal.Clone();

        // 3. Handle downscoping
        if (request.RequestedScopes.Length == 0)
        {
            claimsPrincipal.SetScopes(request.OriginalPrincipal.GetScopes());
        }
        else
        {
            var effectiveScopes = request.RequestedScopes.Intersect(request.OriginalPrincipal.GetScopes()).ToArray();
            logger.LogInformation("User {Subject} and IdP {IdP}. Requested scopes: {RequestedScopes}, Effective scopes: {EffectiveScopes}.",
                subject, idp, request.RequestedScopes, effectiveScopes);
            claimsPrincipal.SetScopes(effectiveScopes);
        }

        return claimsPrincipal;
    }
}
