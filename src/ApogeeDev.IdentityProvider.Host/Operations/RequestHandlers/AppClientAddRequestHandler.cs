using System.Text.Json;
using System.Text.Json.Nodes;
using ApogeeDev.IdentityProvider.Host.Helpers.Authentication;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using MongoDB.Driver;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientAddRequestHandler(OperationContext opContext, ICryptoHelper cryptoHelper, ILogger<AppClientAddRequestHandler> logger)
    : IRequestHandler<AppClientAddRequest, AppClientAddResponse>
{
    public async Task<AppClientAddResponse> Handle(AppClientAddRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Data);

        opContext.AppClientOptions.ThrowIfStaticClient(request.Data.ClientId);

        var collection = await opContext.GetApplicationsCollectionAsync(cancellationToken);

        var builder = Builders<OpenIddictMongoDbApplication>.Filter;

        var existing = await collection.FindAsync(
            builder.Or(
                builder.Eq(doc => doc.ClientId, request.Data.ClientId),
                builder.Eq(doc => doc.DisplayName, request.Data.DisplayName)
            ), null, cancellationToken
        );

        if (await existing.AnyAsync(cancellationToken))
        {
            return new AppClientAddResponse
            {
                Errors = new Dictionary<string, string[]>
                {
                    ["Other"] = [$"Client with name: {request.Data.DisplayName} or Id: {request.Data.ClientId} already exists"],
                }
            };
        }

        var clientSecret = WebEncoders.Base64UrlEncode(
                cryptoHelper.GenerateRandom(40));

        var (permissions, requirements) = request.Data.MapRequirementPermissions();

        var clientType = request.Data.ClientType; // confidential/public
        var applicationType = request.Data.ApplicationType;

        if (request.Data.FlowType == OAuthFlowTypes.ClientCredentials)
        {
            logger.LogInformation("Setting client type to confidential and application type to web for client credentials flow");
            clientType = ClientTypes.Confidential;
            applicationType = ApplicationTypes.Web;
        }

        logger.LogInformation("New client {Permissions}, {Requirements}", permissions, requirements);

        var descriptor = new OpenIddictApplicationDescriptor
        {
            ClientId = request.Data.ClientId,
            ClientSecret = clientSecret,
            DisplayName = request.Data.DisplayName,
            ClientType = clientType, // confidential/public
            ApplicationType = applicationType,
        };

        if(request.Data.FlowType == OAuthFlowTypes.AuthorizationCode)
        {
            descriptor.RedirectUris.UnionWith(request.Data.RedirectUris.Select(s => new Uri(s)));
            descriptor.PostLogoutRedirectUris.UnionWith(request.Data.PostLogoutRedirectUris.Select(s => new Uri(s)));
        }
        descriptor.Permissions.UnionWith(permissions);
        descriptor.Requirements.UnionWith(requirements);

        descriptor.Properties.Add(ConfigureAccessTokenEncryption.SkipTokenEncryptionProp,
            JsonSerializer.SerializeToElement(request.Data.SkipAccessTokenEncryption));

        await opContext.ApplicationManager.CreateAsync(descriptor, cancellationToken);

        return new AppClientAddResponse
        {
            AppClientData = new AppClientDataWithSecret
            {
                ClientSecret = clientSecret,
                ClientId = request.Data.ClientId,
                DisplayName = request.Data.DisplayName,
                ApplicationType = request.Data.ApplicationType,
                ClientType = request.Data.ClientType,
                RedirectUris = request.Data.FlowType == OAuthFlowTypes.AuthorizationCode
                    ? request.Data.RedirectUris : Array.Empty<string>(),
                PostLogoutRedirectUris = request.Data.FlowType == OAuthFlowTypes.AuthorizationCode
                    ? request.Data.PostLogoutRedirectUris : Array.Empty<string>(),
                FlowType = request.Data.FlowType,
                SkipAccessTokenEncryption = request.Data.SkipAccessTokenEncryption,
            }
        };
    }
}

public class AppClientAddResponse
{
    public Dictionary<string, string[]> Errors { get; set; } = default!;
    public AppClientDataWithSecret AppClientData { get; set; } = default!;
}

public class AppClientAddRequest : IRequest<AppClientAddResponse>
{
    public AppClientData Data { get; set; } = default!;
}
