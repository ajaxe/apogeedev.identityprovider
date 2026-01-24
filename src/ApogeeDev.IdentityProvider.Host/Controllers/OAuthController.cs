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
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using ZstdSharp.Unsafe;
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
        var request = HttpContext.GetOpenIddictServerRequest() ??
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

        var result = await HttpContext.AuthenticateAsync();

        if (result is not { Succeeded: true } ||
            request.HasPromptValue(PromptValues.Login) ||
            request.MaxAge is 0 ||
            (request.MaxAge is not null
                && result.Properties?.IssuedUtc is not null
                && DateTimeOffset.UtcNow - result.Properties.IssuedUtc >= TimeSpan.FromSeconds(request.MaxAge.Value)
                && TempData["IgnoreAuthenticationChallenge"] is null or false))
        {
            if (request.HasPromptValue(PromptValues.None))
            {
                return Forbid(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.LoginRequired,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is not logged in."
                    }));
            }

            TempData["IgnoreAuthenticationChallenge"] = true;


            LoginViewModel vm = await mediator.Send(new LoginViewRequest
            {
                AuthorizeRedirectUrl = HttpContext.Request.GetEncodedUrl(),
                ClientId = clientId,
            });

            return View("Login", vm);
        }

        var principal = result.Principal!;

        var signInResult = await mediator.Send(new CreateExternalSignInPrincipalRequest
        {
            IdentityProviderName = principal.Identity?.AuthenticationType
                ?? throw new InvalidOperationException("Invalid 'AuthenticationType' in principal"),
            IncomingExternalPrincipal = principal,
        });

        signInResult.Principal.SetScopes(request.GetScopes());

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
            var scopes = request.GetScopes();
            if (scopes.Length == 0)
            {
                claimsPrincipal.SetScopes(principal.GetScopes());
            }
            else
            {
                claimsPrincipal.SetScopes(scopes.Intersect(principal.GetScopes()));
            }

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

        var user = await dbContext.AppUsers
            .FirstOrDefaultAsync(u => u.Subject == subject && u.IdentityProvider == idp);

        if (user == null)
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

        var userClaims = await dbContext.AppUserClaims
            .Where(u => u.AppUserId == user.Id)
            .ToListAsync();

        // featch all claims from external IdP
        var claims = userClaims.ToDictionary(c => c.ClaimType, c => (object?)c.ClaimValue);

        // change data type of "email_verified" to bool
        if (claims.ContainsKey(Claims.EmailVerified))
        {
            if (bool.TryParse(claims[Claims.EmailVerified]?.ToString(), out var verified))
            {
                claims[Claims.EmailVerified] = verified;
            }
        }

        claims.TryAdd(ClaimTypes.NameIdentifier, user.Subject);
        claims.TryAdd(Claims.Subject, user.Subject);
        claims.TryAdd(Claims.Username, user.UserName);
        claims.TryAdd(ClaimTypes.Name, user.Name);
        claims.TryAdd(Claims.Name, user.Name);
        claims.TryAdd(Claims.Email, user.Email);
        claims.TryAdd(ClaimTypes.Email, user.Email);
        claims.TryAdd(Claims.Picture, user.ProfilePicture);
        claims.TryAdd(CustomClaimTypes.IdpServer.IdP, user.IdentityProvider);

        // Note: the complete list of standard claims supported by the OpenID Connect specification
        // can be found here: http://openid.net/specs/openid-connect-core-1_0.html#StandardClaims

        return Ok(claims);
    }
}
