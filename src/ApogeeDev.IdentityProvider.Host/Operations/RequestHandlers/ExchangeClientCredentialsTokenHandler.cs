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

public class ExchangeClientCredentialsTokenHandler(IMediator mediator)
    : IRequestHandler<ExchangeClientCredentialsTokenRequest, ExchangeClientCredentialsTokenResponse>
{
    public async Task<ExchangeClientCredentialsTokenResponse> Handle(ExchangeClientCredentialsTokenRequest request, CancellationToken cancellationToken)
    {
        var identity = new ClaimsIdentity(
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, // This sets IsAuthenticated = true
            Claims.Name,
            Claims.Role);

        if (string.IsNullOrEmpty(request.ClientId))
        {
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
            return new ExchangeClientCredentialsTokenResponse
            {
                Error = Errors.InvalidClient,
                ErrorDescription = $"Client ID '{clientId}' not found."
            };
        }

        identity.AddClaim(Claims.Subject, clientId, Destinations.AccessToken);
        identity.AddClaim(Claims.Name, response.First().DisplayName, Destinations.AccessToken);

        var claimsPrincipal = new ClaimsPrincipal(identity);

        // Apply requested scopes
        claimsPrincipal.SetScopes(request.RequestedScopes);
        return new ExchangeClientCredentialsTokenResponse { Principal = claimsPrincipal };
    }
}
