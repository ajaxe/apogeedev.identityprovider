using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using ApogeeDev.IdentityProvider.Host.Models.DatabaseModels;

namespace ApogeeDev.IdentityProvider.Host.Helpers;

public static class ClaimsMapper
{
    public static void MapGithubClaims(AppUser user, ClaimsPrincipal principal)
    {
        user.Email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        user.Name = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        user.ProfilePicture = principal.FindFirstValue(CustomClaimTypes.GitHub.AvatarUrl)
            ?? string.Empty;
        user.UserName = principal.FindFirstValue(CustomClaimTypes.GitHub.Login)
            ?? string.Empty;
    }
    public static void MapGoogleClaims(AppUser user, ClaimsPrincipal principal)
    {
        user.Email = principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
        user.Name = principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        user.ProfilePicture = principal.FindFirstValue(CustomClaimTypes.Google.Picture)
            ?? string.Empty;
        user.UserName = principal.FindFirstValue(ClaimTypes.Email)
            ?? string.Empty;
    }
}