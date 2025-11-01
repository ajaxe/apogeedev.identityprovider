using ApogeeDev.IdentityProvider.Host.Operations.Processors;

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
