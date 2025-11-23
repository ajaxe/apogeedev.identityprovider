
using MongoDB.Driver;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientDeleteRequest : IRequest<AppClientDeleteResponse>
{
    public string ClientId { get; set; } = default!;
}

public class AppClientDeleteResponse { }

public class AppClientDeleteRequestHandler : IRequestHandler<AppClientDeleteRequest, AppClientDeleteResponse>
{
    private readonly OperationContext opContext;

    public AppClientDeleteRequestHandler(OperationContext opContext)
    {
        this.opContext = opContext;
    }

    public async Task<AppClientDeleteResponse> Handle(AppClientDeleteRequest request,
        CancellationToken cancellationToken)
    {
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
