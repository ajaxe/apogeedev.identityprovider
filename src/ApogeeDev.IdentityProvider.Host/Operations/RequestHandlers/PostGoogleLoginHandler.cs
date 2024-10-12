using ApogeeDev.IdentityProvider.Host.Operations.Processors;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class GoogleLoginRequest : LoginRequestBase { }

public class PostGoogleLoginHandler : ExtrnalLoginRequestHandlerBase,
    IRequestHandler<GoogleLoginRequest, ExternalLoginResponse>
{
    public PostGoogleLoginHandler(GoogleClaimsProcessor claimsProcessor)
        : base(claimsProcessor) { }

    public Task<ExternalLoginResponse> Handle(GoogleLoginRequest request,
        CancellationToken cancellationToken)
    {
        return base.Handle(request, cancellationToken);
    }
}