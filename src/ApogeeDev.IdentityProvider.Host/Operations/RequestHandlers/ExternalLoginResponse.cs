using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class ExternalLoginResponse
{
    public ClaimsPrincipal Principal { get; set; } = default!;
    public AuthenticationProperties Properties { get; set; } = default!;
}