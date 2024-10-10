using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class CreateExternalSignInPrincipalRequest : IRequest<CreateExternalSignInPrincipalResponse>
{
    public ClaimsPrincipal IncomingExternalPrincipal { get; set; } = default!;
    public string IdentityProviderName { get; set; } = default!;
}

public class CreateExternalSignInPrincipalResponse
{
    public ClaimsPrincipal Principal { get; set; } = default!;
    public AuthenticationProperties? Properties { get; set; }
    public string AuthenticationScheme { get; set; }
        = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
}

public class CreateExternalSignInPrincipalHandler
    : IRequestHandler<CreateExternalSignInPrincipalRequest, CreateExternalSignInPrincipalResponse>
{
    public Task<CreateExternalSignInPrincipalResponse> Handle(CreateExternalSignInPrincipalRequest request,
        CancellationToken cancellationToken)
    {
        var identifier = request.IncomingExternalPrincipal.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        // Create the claims-based identity that will be used by OpenIddict to generate tokens.
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        // Import a few select claims from the identity stored in the local cookie.
        identity.AddClaim(new Claim(Claims.Subject, identifier));
        identity.AddClaim(new Claim(Claims.Name, identifier).SetDestinations(Destinations.AccessToken));
        identity.AddClaim(new Claim(Claims.PreferredUsername, identifier).SetDestinations(Destinations.AccessToken));

        return Task.FromResult(new CreateExternalSignInPrincipalResponse
        {
            Principal = new ClaimsPrincipal(identity),
        });
        
    }
}