using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
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
        var identifier = request.IncomingExternalPrincipal.FindFirst(Claims.Subject)!.Value;

        // Create the claims-based identity that will be used by OpenIddict to generate tokens.
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        identity.AddClaims(request.IncomingExternalPrincipal.Claims);

        identity.SetDestinations(GetDestinations);

        return Task.FromResult(new CreateExternalSignInPrincipalResponse
        {
            Principal = new ClaimsPrincipal(identity),
        });
    }

    private static IEnumerable<string> GetDestinations(Claim claim)
    {
        // Note: by default, claims are NOT automatically included in the access and identity tokens.
        // To allow OpenIddict to serialize them, you must attach them a destination, that specifies
        // whether they should be included in access tokens, in identity tokens or in both.

        switch (claim.Type)
        {
            case Claims.Name or Claims.PreferredUsername:
                yield return Destinations.AccessToken;

                if (claim.Subject?.HasScope(Scopes.Profile) == true)
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Email:
                yield return Destinations.AccessToken;

                if (claim.Subject?.HasScope(Scopes.Email) == true)
                    yield return Destinations.IdentityToken;

                yield break;

            case Claims.Role:
                yield return Destinations.AccessToken;

                if (claim.Subject?.HasScope(Scopes.Roles) == true)
                    yield return Destinations.IdentityToken;

                yield break;

            // Never include the security stamp in the access and identity tokens, as it's a secret value.
            case "AspNet.Identity.SecurityStamp": yield break;

            case CustomClaimTypes.IdpServer.IdP:
                yield return Destinations.AccessToken;
                yield return Destinations.IdentityToken;
                yield break;

            default:
                yield return Destinations.AccessToken;
                yield break;
        }
    }
}
