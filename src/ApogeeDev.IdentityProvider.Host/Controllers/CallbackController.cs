using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace ApogeeDev.IdentityProvider.Host.Controllers;

[ApiController]
[Route("[controller]")]
public class CallbackController : ControllerBase
{
    private readonly ILogger<CallbackController> logger;
    private readonly IMediator mediator;

    public CallbackController(ILogger<CallbackController> logger,
        IMediator mediator)
    {
        this.logger = logger;
        this.mediator = mediator;
    }

    [HttpPost("login/github")]
    [HttpGet("login/github")]
    public async Task<IActionResult> LogInWithGithub(string? returnUrl)
    {
        var result = await HttpContext.AuthenticateAsync(Providers.GitHub);

        if (result.Principal is not ClaimsPrincipal { Identity.IsAuthenticated: true })
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }

        var loginResponse = await mediator.Send(new GithubLoginRequest
        {
            LoginResult = result,
        });

        // For scenarios where the default sign-in handler configured in the ASP.NET Core
        // authentication options shouldn't be used, a specific scheme can be specified here.
        return SignIn(loginResponse.Principal, loginResponse!.Properties);
    }
    [HttpPost("~/callback/login/google")]
    [HttpGet("~/callback/login/google")]
    public async Task<IActionResult> LogInWithGoogle(string? returnUrl)
    {
        var result = await HttpContext.AuthenticateAsync(Providers.Google);

        if (result.Principal is not ClaimsPrincipal { Identity.IsAuthenticated: true })
        {
            throw new InvalidOperationException("The external authorization data cannot be used for authentication.");
        }

        var loginResponse = await mediator.Send(new GoogleLoginRequest
        {
            LoginResult = result,
        });

        // For scenarios where the default sign-in handler configured in the ASP.NET Core
        // authentication options shouldn't be used, a specific scheme can be specified here.
        return SignIn(loginResponse.Principal, loginResponse!.Properties);
    }
}