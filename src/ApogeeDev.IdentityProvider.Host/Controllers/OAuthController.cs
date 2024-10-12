using ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace ApogeeDev.IdentityProvider.Host.Controllers;

[ApiController]
[Route("connect")]
public class OAuthController : Controller
{
    [HttpGet("authorize")]
    [HttpPost("authorize")]
    public async Task<IActionResult> Authorize([FromQuery(Name = "client_id")] string clientId,
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
            var vm = await mediator.Send(new LoginViewRequest
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
    public IActionResult AuthorizeWithGoogle(string redirectUrl,
        [FromServices] ICryptoHelper cryptoHelper)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = cryptoHelper.DecryptAsBase64Url(redirectUrl),
        };

        return Challenge(properties, [Providers.GitHub]);
    }

    [HttpGet("logout")]
    [HttpPost("logout")]
    [IgnoreAntiforgeryToken]
    public IActionResult Logout()
    {
        return SignOut(
            authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
            properties: new AuthenticationProperties
            {
                RedirectUri = "/"
            });
    }
}
