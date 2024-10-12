using System.Security.Claims;

namespace ApogeeDev.IdentityProvider.Host.Operations.Processors;

public class ClaimsProcessorResult
{
    public ClaimsProcessorResult()
    {
        IdClaims = Enumerable.Empty<Claim>();
    }

    public IEnumerable<Claim> IdClaims { get; set; }
}