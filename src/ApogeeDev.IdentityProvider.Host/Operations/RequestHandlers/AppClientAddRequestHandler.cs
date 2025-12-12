using Microsoft.AspNetCore.WebUtilities;
using MongoDB.Driver;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientAddRequestHandler(OperationContext opContext, ICryptoHelper cryptoHelper)
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

        var data = new OpenIddictMongoDbApplication
        {
            ClientId = request.Data.ClientId,
            ClientSecret = clientSecret,
            DisplayName = request.Data.DisplayName,
            ApplicationType = request.Data.ApplicationType,
            ClientType = request.Data.ClientType,
            RedirectUris = request.Data.RedirectUris,
            PostLogoutRedirectUris = request.Data.PostLogoutRedirectUris,
        };

        await collection.InsertOneAsync(data, options: null, cancellationToken);

        return new AppClientAddResponse
        {
            AppClientData = new AppClientDataWithSecret
            {
                ClientSecret = clientSecret,
                ClientId = request.Data.ClientId,
                DisplayName = request.Data.DisplayName,
                ApplicationType = request.Data.ApplicationType,
                ClientType = request.Data.ClientType,
                RedirectUris = request.Data.RedirectUris,
                PostLogoutRedirectUris = request.Data.PostLogoutRedirectUris,
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
