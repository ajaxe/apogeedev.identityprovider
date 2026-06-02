using System.Text.Json;
using ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;
using OpenIddict.Server;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace ApogeeDev.IdentityProvider.Host.Helpers.Authentication;

public class ConfigureAccessTokenEncryption(OperationContext opContext, ILogger<ConfigureAccessTokenEncryption> logger)
    : IOpenIddictServerHandler<ProcessSignInContext>
{
    public const string SkipTokenEncryptionProp = "SkipTokenEncryption";
    public async ValueTask HandleAsync(ProcessSignInContext context)
    {
        // 1. Target only access tokens
        if (context.AccessTokenPrincipal is not null)
        {
            // 2. Inspect the Client ID attached to the request
            string? clientId = context.Request.ClientId;

            if(string.IsNullOrWhiteSpace(clientId))
            {
                logger.LogInformation("Client ID is missing in the token request. Skipping client-specific encryption configuration.");
                return;
            }

            var app = await opContext.ApplicationManager.FindByClientIdAsync(clientId, context.CancellationToken);

            // 3. Dynamically alter encryption behavior for specific clients
            if (app is not null)
            {
                var properties = await opContext.ApplicationManager.GetPropertiesAsync(app, context.CancellationToken);
                if(properties.TryGetValue(SkipTokenEncryptionProp, out var skipEncryptionValue) &&
                    skipEncryptionValue is JsonElement jsonElement &&
                    jsonElement.ValueKind == JsonValueKind.True)
                {
                    logger.LogInformation("Client {ClientId} is configured to skip access token encryption.", clientId);
                    context.Options.DisableAccessTokenEncryption = true; // Disable access token encryption for this client
                }
                else
                {
                    logger.LogInformation("Client {ClientId} is not configured to skip access token encryption. Using default behavior.", clientId);
                }
            }
        }
    }
}
