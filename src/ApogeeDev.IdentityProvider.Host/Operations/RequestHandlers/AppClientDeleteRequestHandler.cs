
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientDeleteRequest : IRequest<AppClientDeleteResponse>
{
    public string ClientId { get; set; } = default!;
}

public class AppClientDeleteResponse { }

public class AppClientDeleteRequestHandler(OperationContext opContext) : IRequestHandler<AppClientDeleteRequest, AppClientDeleteResponse>
{
    public async Task<AppClientDeleteResponse> Handle(AppClientDeleteRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.ClientId);

        opContext.AppClientOptions.ThrowIfStaticClient(request.ClientId);

        var collection = await opContext.GetApplicationsCollectionAsync(cancellationToken);

        var builder = Builders<OpenIddictMongoDbApplication>.Filter;

        var result = await collection.DeleteOneAsync(
            builder.Eq(doc => doc.ClientId, request.ClientId),
            options: null,
            cancellationToken
        );

        return new AppClientDeleteResponse();
    }
}
