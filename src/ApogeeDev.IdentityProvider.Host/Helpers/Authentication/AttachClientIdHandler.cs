using OpenIddict.Abstractions;
using OpenIddict.Server;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace ApogeeDev.IdentityProvider.Host.Helpers.Authentication;

public class AttachClientIdHandler(ILogger<AttachClientIdHandler> logger)
    : IOpenIddictServerHandler<ProcessSignInContext>
{
    public ValueTask HandleAsync(ProcessSignInContext context)
    {
        string? clientId = context.Request?.ClientId;

        if (string.IsNullOrWhiteSpace(clientId))
        {
            logger.LogInformation("Client ID is missing in token request. Skipping client_id claim attachment.");
            return ValueTask.CompletedTask;
        }

        if (context.AccessTokenPrincipal is not null)
        {
            if (!context.AccessTokenPrincipal.HasClaim(Claims.ClientId, clientId))
            {
                logger.LogInformation("Attaching Client ID {ClientId} as '{ClaimType}' claim to AccessTokenPrincipal.",
                    clientId, Claims.ClientId);
                context.AccessTokenPrincipal.SetClaim(Claims.ClientId, clientId);
            }

            if (!context.AccessTokenPrincipal.GetAudiences().Any())
            {
                logger.LogInformation("Setting default audience (aud) to Client ID {ClientId} on AccessTokenPrincipal.",
                    clientId);
                context.AccessTokenPrincipal.SetAudiences(clientId);
            }
        }

        return ValueTask.CompletedTask;
    }
}
