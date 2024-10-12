using System.Security.Claims;
using ApogeeDev.IdentityProvider.Host.Data;
using ApogeeDev.IdentityProvider.Host.Models.DatabaseModels;
using Microsoft.EntityFrameworkCore;
using static OpenIddict.Client.WebIntegration.OpenIddictClientWebIntegrationConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.Processors;

public abstract class ClaimsProcessorBase : IClaimsProcessor
{
    protected ClaimsProcessorBase(ApplicationDbContext dbContext,
        ILogger logger)
    {
        this.dbContext = dbContext;
        this.logger = logger;
    }

    protected ApplicationDbContext dbContext { get; }
    protected ILogger logger { get; }
    protected virtual Dictionary<string, string> IdClaimMap => throw new NotImplementedException();

    public virtual async Task<ClaimsProcessorResult> Process(ClaimsPrincipal principal)
    {
        var idClaims = GetIdentityClaims(principal);
        await PersistClaims(principal, idClaims);

        return new ClaimsProcessorResult
        {
            IdClaims = idClaims.Select(c => new Claim(IdClaimMap[c.Type], c.Value))
        };
    }

    protected virtual IEnumerable<Claim> GetIdentityClaims(ClaimsPrincipal principal)
    {
        foreach (var claimType in IdClaimMap.Keys)
        {
            var existing = principal.FindFirst(c => c.Type == claimType);
            if (existing is not null)
            {
                yield return existing;
            }
            else logger.LogWarning($"claim type: '{claimType}' not present. name: '{principal.Identity?.Name}'");
        }
    }
    protected virtual async Task PersistClaims(ClaimsPrincipal principal, IEnumerable<Claim> idClaims)
    {
        var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException($"'NameIdentifier' claim missing");

        var user = await dbContext.AppUsers.FirstOrDefaultAsync(
            u => u.Subject == subject && u.IdentityProvider == Providers.GitHub);

        if (user == null)
        {
            user = AppUser.Create(subject, Providers.GitHub, principal);
            dbContext.AppUsers.Add(user);
        }
        else
        {
            user.ApplyClaims(principal);
        }

        await dbContext.SaveChangesAsync();

        var toDelete = dbContext.AppUserClaims
            .Where(uc => uc.AppUserId == user.Id);

        dbContext.AppUserClaims.RemoveRange(toDelete);

        var nonIdClaims = principal.Claims.Except(idClaims, (x, y) => x.Type == y.Type);

        await dbContext.AppUserClaims.AddRangeAsync(
            nonIdClaims.Select(c => new AppUserClaim
            {
                AppUserId = user.Id,
                ClaimType = c.Type,
                ClaimValue = c.Value,
            }));

        await dbContext.SaveChangesAsync();
    }
}