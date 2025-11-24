using MongoDB.Driver;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientUpdateRequestHandler : IRequestHandler<AppClientUpdateRequest, AppClientUpdateResponse>
{
    private readonly OperationContext opContext;

    public AppClientUpdateRequestHandler(OperationContext opContext)
    {
        this.opContext = opContext;
    }

    public async Task<AppClientUpdateResponse> Handle(AppClientUpdateRequest request,
        CancellationToken cancellationToken)
    {
        if(request.Data == null)
        {
            throw new ArgumentNullException(nameof(request.Data));
        }
        if(string.IsNullOrWhiteSpace(request.ClientId))
        {
            throw new ArgumentNullException(nameof(request.ClientId));
        }
        if(request.ClientId != request.Data.ClientId)
        {
            throw new ArgumentException("ClientId in URL and body do not match");
        }

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
