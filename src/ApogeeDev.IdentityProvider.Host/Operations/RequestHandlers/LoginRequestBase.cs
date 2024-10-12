using Microsoft.AspNetCore.Authentication;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;
public abstract class LoginRequestBase : IRequest<ExternalLoginResponse>
{
    public AuthenticateResult LoginResult { get; set; } = default!;
}