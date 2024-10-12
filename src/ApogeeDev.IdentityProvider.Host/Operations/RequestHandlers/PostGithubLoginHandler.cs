using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Data;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using ApogeeDev.IdentityProvider.Host.Models.DatabaseModels;
using ApogeeDev.IdentityProvider.Host.Operations.Processors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using ZstdSharp.Unsafe;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class GithubLoginRequest : IRequest<GithubLoginResponse>
{
    public AuthenticateResult LoginResult { get; set; } = default!;
}
public class GithubLoginResponse
{
    public ClaimsPrincipal Principal { get; set; } = default!;
    public AuthenticationProperties Properties { get; set; } = default!;
}
public class PostGithubLoginHandler : IRequestHandler<GithubLoginRequest, GithubLoginResponse>
{
    private readonly GithubClaimsProcessor claimsProcessor;

    public PostGithubLoginHandler(GithubClaimsProcessor claimsProcessor)
    {
        this.claimsProcessor = claimsProcessor;
    }

    public async Task<GithubLoginResponse> Handle(GithubLoginRequest request, CancellationToken cancellationToken)
    {
        var result = request.LoginResult;

        _ = result.Principal ?? throw new InvalidOperationException("Invlaid 'principal' in request.");

        var identity = new ClaimsIdentity(
            authenticationType: Providers.GitHub,
            nameType: ClaimTypes.Name,
            roleType: ClaimTypes.Role);

        var r = await claimsProcessor.Process(result.Principal);

        identity.AddClaims(r.IdClaims);

        var properties = new AuthenticationProperties(result.Properties!.Items)
        {
            RedirectUri = result.Properties!.RedirectUri
        };

        return new GithubLoginResponse
        {
            Principal = new ClaimsPrincipal(identity),
            Properties = properties
        };
    }
}
