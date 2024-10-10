using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace ApogeeDev.IdentityProvider.Host.Controllers;

[ApiController]
[Route("[controller]")]
public class CallbackController : ControllerBase
{
    private readonly ILogger<CallbackController> logger;

    public CallbackController(ILogger<CallbackController> logger)
    {
        this.logger = logger;
    }

    [HttpPost("login/github")]
    [HttpGet("login/github")]
    public async Task<IActionResult> LogInWithGithub(string? returnUrl)
    {
        var result = await HttpContext.AuthenticateAsync(Providers.GitHub);

        LogClaims(Providers.GitHub, result);

        var identity = new ClaimsIdentity(
            authenticationType: Providers.GitHub,
            nameType: ClaimTypes.Name,
            roleType: ClaimTypes.Role);

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.Principal!.FindFirst("id")!.Value));

        var properties = new AuthenticationProperties
        {
            RedirectUri = result.Properties!.RedirectUri
        };

        // For scenarios where the default sign-in handler configured in the ASP.NET Core
        // authentication options shouldn't be used, a specific scheme can be specified here.
        return SignIn(new ClaimsPrincipal(identity), properties);
    }
    [HttpPost("~/callback/login/google")]
    [HttpGet("~/callback/login/google")]
    public async Task<IActionResult> LogInWithGoogle(string returnUrl)
    {
        var result = await HttpContext.AuthenticateAsync(Providers.Google);

        LogClaims(Providers.Google, result);

        var identity = new ClaimsIdentity(
            authenticationType: Providers.Google,
            nameType: ClaimTypes.Name,
            roleType: ClaimTypes.Role);

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.Principal!.FindFirst("id")!.Value));

        var properties = new AuthenticationProperties
        {
            RedirectUri = result.Properties!.RedirectUri
        };

        // For scenarios where the default sign-in handler configured in the ASP.NET Core
        // authentication options shouldn't be used, a specific scheme can be specified here.
        return SignIn(new ClaimsPrincipal(identity), properties);
    }

    private void LogClaims(string providerType, AuthenticateResult result)
    {
        logger.LogInformation("{@Provider} {@Claims}", providerType,
             result.Principal!.Claims.Select(c => new { c.Type, c.Value }));
    }
}