using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace ApogeeDev.IdentityProvider.Host.Controllers;

[ApiController]
[Route("connect")]
public class OAuthController : ControllerBase
{
    [HttpGet("authorize")]
    [HttpPost("authorize")]
    public async Task<IActionResult> Authorize()
    {
        // Resolve the claims stored in the cookie created after the GitHub authentication dance.
        // If the principal cannot be found, trigger a new challenge to redirect the user to GitHub.
        //
        // For scenarios where the default authentication handler configured in the ASP.NET Core
        // authentication options shouldn't be used, a specific scheme can be specified here.
        var principal = (await HttpContext.AuthenticateAsync())?.Principal;
        if (principal is null)
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = HttpContext.Request.GetEncodedUrl()
            };

            return Challenge(properties, [Providers.GitHub]);
        }

        var identifier = principal.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        // Create the claims-based identity that will be used by OpenIddict to generate tokens.
        var identity = new ClaimsIdentity(
            authenticationType: TokenValidationParameters.DefaultAuthenticationType,
            nameType: Claims.Name,
            roleType: Claims.Role);

        // Import a few select claims from the identity stored in the local cookie.
        identity.AddClaim(new Claim(Claims.Subject, identifier));
        identity.AddClaim(new Claim(Claims.Name, identifier).SetDestinations(Destinations.AccessToken));
        identity.AddClaim(new Claim(Claims.PreferredUsername, identifier).SetDestinations(Destinations.AccessToken));

        return SignIn(new ClaimsPrincipal(identity), properties: null,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
    }
}
