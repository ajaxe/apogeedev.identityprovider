using System.Security.Claims;

namespace ApogeeDev.IdentityProvider.Host.Operations.Processors;

public interface IClaimsProcessor
{
    public Task<ClaimsProcessorResult> Process(ClaimsPrincipal principal);
}