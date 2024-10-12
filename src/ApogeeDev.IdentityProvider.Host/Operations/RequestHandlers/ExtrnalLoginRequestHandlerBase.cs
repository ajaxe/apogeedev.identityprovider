using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Operations.Processors;
using Microsoft.AspNetCore.Authentication;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public abstract class ExtrnalLoginRequestHandlerBase
{
    private readonly IClaimsProcessor claimsProcessor;

    public ExtrnalLoginRequestHandlerBase(IClaimsProcessor claimsProcessor)
    {
        this.claimsProcessor = claimsProcessor;
    }
    protected async Task<ExternalLoginResponse> Handle(LoginRequestBase request,
        CancellationToken cancellationToken)
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

        return new ExternalLoginResponse
        {
            Principal = new ClaimsPrincipal(identity),
            Properties = properties
        };
    }
}