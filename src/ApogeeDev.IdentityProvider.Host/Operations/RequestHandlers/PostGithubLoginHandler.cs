using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Operations.Processors;
using Microsoft.AspNetCore.Authentication;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class GithubLoginRequest : LoginRequestBase
{

}

public class PostGithubLoginHandler : ExtrnalLoginRequestHandlerBase,
    IRequestHandler<GithubLoginRequest, ExternalLoginResponse>
{
    public PostGithubLoginHandler(GithubClaimsProcessor claimsProcessor)
        : base(claimsProcessor) { }

    public Task<ExternalLoginResponse> Handle(GithubLoginRequest request,
        CancellationToken cancellationToken)
    {
        return base.Handle(request, cancellationToken);
    }
}
