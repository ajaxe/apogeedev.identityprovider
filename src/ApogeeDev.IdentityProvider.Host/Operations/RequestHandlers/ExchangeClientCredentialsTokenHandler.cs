using System.Security.Claims;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class ExchangeClientCredentialsTokenRequest : IRequest<ExchangeClientCredentialsTokenResponse>
{
    public string? ClientId { get; set; } = null!;
    public string[] RequestedScopes { get; set; } = Array.Empty<string>();
}

public class ExchangeClientCredentialsTokenResponse
{
    public ClaimsPrincipal? Principal { get; set; }
    public string? Error { get; set; }
    public string? ErrorDescription { get; set; }
}

public class ExchangeClientCredentialsTokenHandler(IMediator mediator, ILogger<ExchangeClientCredentialsTokenHandler> logger)
    : IRequestHandler<ExchangeClientCredentialsTokenRequest, ExchangeClientCredentialsTokenResponse>
{
    public async Task<ExchangeClientCredentialsTokenResponse> Handle(ExchangeClientCredentialsTokenRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.ClientId))
        {
            logger.LogWarning("Client ID is missing during client credentials token exchange.");
            return new ExchangeClientCredentialsTokenResponse
            {
                Error = Errors.InvalidClient,
                ErrorDescription = "Client ID is missing."
            };
        }

        // Client Credentials must map back to the Application Client ID as the Subject
        var clientId = request.ClientId;

        var response = await mediator.Send(new AppClientListRequest
        {
            ClientId = clientId,
        }, cancellationToken);

        if (response.Count == 0)
        {
            logger.LogWarning("Client ID '{ClientId}' not found during client credentials token exchange.", clientId);
            return new ExchangeClientCredentialsTokenResponse
            {
                Error = Errors.InvalidClient,
                ErrorDescription = $"Client ID '{clientId}' not found."
            };
        }

        var clientDisplayName = response.First().DisplayName;

        var identity = new ClaimsIdentity(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, // This sets IsAuthenticated = true
            Claims.Name,
            Claims.Role);

        logger.LogInformation("Adding claims {Subject} and {Name}", clientId, clientDisplayName);

        identity.AddClaim(Claims.Subject, clientId, Destinations.AccessToken);
        identity.AddClaim(Claims.Name, clientDisplayName, Destinations.AccessToken);

        var claimsPrincipal = new ClaimsPrincipal(identity);

        logger.LogInformation("{ClientDisplayName} scopes: {RequestedScopes}",
            clientDisplayName, request.RequestedScopes);

        // Apply requested scopes
        claimsPrincipal.SetScopes(request.RequestedScopes);
        return new ExchangeClientCredentialsTokenResponse { Principal = claimsPrincipal };
    }
}
