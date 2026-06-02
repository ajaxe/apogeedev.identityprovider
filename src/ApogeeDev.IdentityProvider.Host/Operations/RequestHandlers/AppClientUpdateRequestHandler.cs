using System.Text.Json;
using ApogeeDev.IdentityProvider.Host.Helpers.Authentication;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using OpenIddict.MongoDb.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientUpdateRequestHandler(OperationContext opContext, ILogger<AppClientUpdateRequestHandler> logger)
    : IRequestHandler<AppClientUpdateRequest, AppClientUpdateResponse>
{
    public async Task<AppClientUpdateResponse> Handle(AppClientUpdateRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ArgumentNullException.ThrowIfNull(request.Data);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.ClientId);

        if (request.ClientId != request.Data.ClientId)
        {
            throw new ArgumentException("ClientId in URL and body do not match");
        }

        opContext.AppClientOptions.ThrowIfStaticClient(request.ClientId);

        var collection = await opContext.GetApplicationsCollectionAsync(cancellationToken);

        var (permissions, requirements) = request.Data.MapRequirementPermissions();

        var clientType = request.Data.ClientType; // confidential/public
        var applicationType = request.Data.ApplicationType;
        var redirectUris = request.Data.RedirectUris;
        var postLogoutRedirectUris = request.Data.PostLogoutRedirectUris;

        if (request.Data.FlowType == OAuthFlowTypes.ClientCredentials)
        {
            logger.LogInformation("Setting client type to confidential and application type to web for client credentials flow");
            clientType = ClientTypes.Confidential;
            applicationType = ApplicationTypes.Web;
            logger.LogInformation("Removing redirect uris for client credentials flow");
            redirectUris = Array.Empty<string>();
            postLogoutRedirectUris = Array.Empty<string>();
        }

        logger.LogInformation("Updating client {Permissions}, {Requirements}", permissions, requirements);

        var propKey = ConfigureAccessTokenEncryption.SkipTokenEncryptionProp;
        var propValue = BsonValue.Create(request.Data.SkipAccessTokenEncryption);

        var updateDef = Builders<OpenIddictMongoDbApplication>.Update
            .Set(doc => doc.DisplayName, request.Data.DisplayName)
            .Set(doc => doc.ApplicationType, applicationType)
            .Set(doc => doc.ClientType, clientType)
            .Set(doc => doc.RedirectUris, redirectUris)
            .Set(doc => doc.PostLogoutRedirectUris, postLogoutRedirectUris)
            .Set(doc => doc.Permissions, permissions)
            .Set(doc => doc.Requirements, requirements)
            .Set($"{nameof(OpenIddictMongoDbApplication.Properties)}.{propKey}", propValue);

        var result = await collection.UpdateOneAsync(
            Builders<OpenIddictMongoDbApplication>.Filter.Eq(doc => doc.ClientId, request.Data.ClientId),
            updateDef,
            null, // UpdateOptions
            cancellationToken);

        return new AppClientUpdateResponse();
    }
}

public class AppClientUpdateRequest : IRequest<AppClientUpdateResponse>
{
    public string ClientId { get; set; } = default!;
    public AppClientData Data { get; set; } = default!;
}

public class AppClientUpdateResponse
{
    public Dictionary<string, string[]> Errors { get; set; } = default!;
}
