using OpenIddict.Abstractions;
using OpenIddict.Server;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace ApogeeDev.IdentityProvider.Host.Helpers.Authentication;

public class AttachClientAudienceHandler(ILogger<AttachClientAudienceHandler> logger)
    : IOpenIddictServerHandler<ProcessSignInContext>
{
    public ValueTask HandleAsync(ProcessSignInContext context)
    {
        string? clientId = context.Request?.ClientId;

        if (string.IsNullOrWhiteSpace(clientId))
        {
            logger.LogInformation("Client ID is missing in token request. Skipping audience attachment.");
            return ValueTask.CompletedTask;
        }

        if (context.AccessTokenPrincipal is not null)
        {
            var currentAudiences = context.AccessTokenPrincipal.GetAudiences().ToList();
            if (!currentAudiences.Contains(clientId))
            {
                logger.LogInformation("Attaching Client ID {ClientId} as audience to AccessTokenPrincipal.", clientId);
                context.AccessTokenPrincipal.SetAudiences(currentAudiences.Append(clientId));
            }
        }

        if (context.IdentityTokenPrincipal is not null)
        {
            var currentAudiences = context.IdentityTokenPrincipal.GetAudiences().ToList();
            if (!currentAudiences.Contains(clientId))
            {
                logger.LogInformation("Attaching Client ID {ClientId} as audience to IdentityTokenPrincipal.", clientId);
                context.IdentityTokenPrincipal.SetAudiences(currentAudiences.Append(clientId));
            }
        }

        return ValueTask.CompletedTask;
    }
}
