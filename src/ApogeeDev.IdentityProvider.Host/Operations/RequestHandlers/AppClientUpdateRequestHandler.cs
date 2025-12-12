using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientUpdateRequestHandler(OperationContext opContext)
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

        var result = await collection.UpdateOneAsync(
            Builders<OpenIddictMongoDbApplication>.Filter.Eq(doc => doc.ClientId, request.Data.ClientId),
            Builders<OpenIddictMongoDbApplication>.Update
                .Set(doc => doc.DisplayName, request.Data.DisplayName)
                .Set(doc => doc.ApplicationType, request.Data.ApplicationType)
                .Set(doc => doc.ClientType, request.Data.ClientType)
                .Set(doc => doc.RedirectUris, request.Data.RedirectUris)
                .Set(doc => doc.PostLogoutRedirectUris, request.Data.PostLogoutRedirectUris),
            null,
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
