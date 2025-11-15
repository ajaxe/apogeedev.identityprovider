using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace ApogeeDev.IdentityProvider.Host.Helpers;

public class AppManagerClaimsTranformation : IClaimsTransformation
{
    private readonly AppOptions appOptions;

    public AppManagerClaimsTranformation(IOptionsSnapshot<AppOptions> options)
    {
        appOptions = options.Value;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity();

        var email = principal.GetClaim(OpenIddictConstants.Claims.Email);

        if (appOptions.AppManagerEmails.Contains(email, StringComparer.OrdinalIgnoreCase))
        {
            claimsIdentity.AddClaim(new Claim(CustomClaimTypes.Common.AppManager, "1"));
        }

        principal.AddIdentity(claimsIdentity);

        return Task.FromResult(principal);
    }
}
