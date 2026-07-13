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

public class ExchangeRefreshTokenRequestHandler : IRequestHandler<ExchangeRefreshTokenRequest, ClaimsPrincipal?>
{
    private readonly ApplicationDbContext _dbContext;

    public ExchangeRefreshTokenRequestHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClaimsPrincipal?> Handle(ExchangeRefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var subject = request.OriginalPrincipal.GetClaim(Claims.Subject);
        var idp = request.OriginalPrincipal.GetClaim(CustomClaimTypes.IdpServer.IdP);

        // 1. Verify user still exists
        var user = await _dbContext.AppUsers
            .FirstOrDefaultAsync(u => u.Subject == subject && u.IdentityProvider == idp, cancellationToken);

        if (user == null)
        {
            // Returning null signals to the controller that the user is invalid/deleted
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
            claimsPrincipal.SetScopes(request.RequestedScopes.Intersect(request.OriginalPrincipal.GetScopes()));
        }

        return claimsPrincipal;
    }
}
