using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Data;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace ApogeeDev.IdentityProvider.Host.Controllers;

[ApiController]
[Route("connect")]
public class OAuthController : Controller
{
    [HttpGet("authorize")]
    [HttpPost("authorize")]
    public async Task<IActionResult> OAuthAuthorize([FromQuery(Name = "client_id")] string clientId,
        [FromServices] IMediator mediator)
    {
        // Resolve the claims stored in the cookie created after the GitHub authentication dance.
        // If the principal cannot be found, trigger a new challenge to redirect the user to GitHub.
        //
        // For scenarios where the default authentication handler configured in the ASP.NET Core
        // authentication options shouldn't be used, a specific scheme can be specified here.
        var principal = (await HttpContext.AuthenticateAsync())?.Principal;

        if (principal is null)
        {
            LoginViewModel vm = await mediator.Send(new LoginViewRequest
            {
                AuthorizeRedirectUrl = HttpContext.Request.GetEncodedUrl(),
                ClientId = clientId,
            });

            return View("Login", vm);
        }

        var signInResult = await mediator.Send(new CreateExternalSignInPrincipalRequest
        {
            IdentityProviderName = principal.Identity?.AuthenticationType
                ?? throw new InvalidOperationException("Invalid 'AuthenticationType' in principal"),
            IncomingExternalPrincipal = principal,
        });

        return SignIn(signInResult.Principal,
            properties: signInResult.Properties!,
            signInResult.AuthenticationScheme);
    }

    [HttpPost("authorize/github", Name = "AuthorizeWithGithub")]
    [RequireAntiforgeryToken]
    public IActionResult AuthorizeWithGithub([FromForm] string redirectUrl,
        [FromServices] ICryptoHelper cryptoHelper)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = cryptoHelper.DecryptAsBase64Url(redirectUrl),
        };

        return Challenge(properties, [Providers.GitHub]);
    }
    [HttpPost("authorize/google", Name = "AuthorizeWithGoogle")]
    [RequireAntiforgeryToken]
    public IActionResult AuthorizeWithGoogle([FromForm] string redirectUrl,
        [FromServices] ICryptoHelper cryptoHelper)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = cryptoHelper.DecryptAsBase64Url(redirectUrl),
        };

        return Challenge(properties, [Providers.Google]);
    }

    [HttpGet("logout")]
    [HttpPost("logout")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Logout()
    {
        var request = HttpContext.GetOpenIddictServerRequest();

        await HttpContext.SignOutAsync();

        return SignOut(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties
            {
                RedirectUri = request?.PostLogoutRedirectUri ?? "/",
            });
    }

    [HttpPost("token")]
    public async Task<IActionResult> Exchange()
    {
        var request = HttpContext.GetOpenIddictServerRequest()
            ?? throw new InvalidOperationException("Invalid open id request");

        // Handle the authorization code grant type
        if (request.IsAuthorizationCodeGrantType() || request.IsRefreshTokenGrantType())
        {
            // Retrieve the claims principal associated with the authorization code
            var principal = (await HttpContext.AuthenticateAsync(
                OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)).Principal;

            // Set the appropriate scopes
            var claimsPrincipal = principal!.Clone();
            claimsPrincipal.SetScopes(request.GetScopes());

            return SignIn(claimsPrincipal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        return BadRequest("Invalid grant type.");
    }

    [Authorize(AuthenticationSchemes = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("~/connect/userinfo")]
    [HttpPost("~/connect/userinfo")]
    [Produces("application/json")]
    public async Task<IActionResult> Userinfo([FromServices] ApplicationDbContext dbContext)
    {
        var subject = User.GetClaim(Claims.Subject);
        var idp = User.GetClaim(CustomClaimTypes.IdpServer.IdP);

        var found = await dbContext.AppUsers
            .CountAsync(u => u.Subject == subject && u.IdentityProvider == idp);

        if (found <= 0)
        {
            return Challenge(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] =
                        "The specified access token is bound to an account that no longer exists."
                }));
        }

        var user = await dbContext.AppUsers
            .FirstAsync(u => u.Subject == subject && u.IdentityProvider == idp);

        var claims = new Dictionary<string, object?>(StringComparer.Ordinal)
        {
            // Note: the "sub" claim is a mandatory claim and must be included in the JSON response.
            [ClaimTypes.NameIdentifier] = user.Subject,
            [Claims.Subject] = user.Subject,
            [Claims.Username] = user.UserName,
            [ClaimTypes.Name] = user.Name,
            [Claims.Name] = user.Name,
            [Claims.Email] = user.Email,
            [ClaimTypes.Email] = user.Email,
            [Claims.Picture] = user.ProfilePicture,
            [CustomClaimTypes.IdpServer.IdP] = user.IdentityProvider,
        };

        // Note: the complete list of standard claims supported by the OpenID Connect specification
        // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

        return Ok(claims);
    }
}
