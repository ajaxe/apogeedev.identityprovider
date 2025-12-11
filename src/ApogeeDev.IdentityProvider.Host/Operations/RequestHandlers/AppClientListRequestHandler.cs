using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientListRequestHandler(OperationContext opContext,
    IOptionsSnapshot<AppClientOptions> options)
    : IRequestHandler<AppClientListRequest, AppClientListResponse>
{
    private readonly IOpenIddictMongoDbContext dbContext = opContext.DbContext;
    private readonly OpenIddictMongoDbOptions options = opContext.Options;
    private readonly AppClientOptions appClientOptions = options.Value;

    public async Task<AppClientListResponse> Handle(AppClientListRequest request,
        CancellationToken cancellationToken)
    {
        var database = await dbContext.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<OpenIddictMongoDbApplication>(options.ApplicationsCollectionName);

        var query = collection.AsQueryable();

        if (string.IsNullOrWhiteSpace(request.ClientId))
            query = query.OrderBy(x => (x.DisplayName ?? string.Empty).ToUpper());
        else query = query.Where(x => x.ClientId == request.ClientId);

        var clients = await query
            .Select(app => new AppClientData
            {
                DisplayName = app.DisplayName ?? string.Empty,
                ClientId = app.ClientId ?? string.Empty,
                ApplicationType = app.ApplicationType ?? string.Empty,
                ClientType = app.ClientType ?? string.Empty,
                RedirectUris = app.RedirectUris == null ? new string[0] : app.RedirectUris.ToArray(),
                PostLogoutRedirectUris = app.PostLogoutRedirectUris == null ? new string[0] : app.PostLogoutRedirectUris.ToArray(),
            })
            .ToListAsync(cancellationToken);

        foreach (var itm in clients.IntersectBy(appClientOptions.Clients.Select(x => x.ClientId),
                                                    (v) => v.ClientId))
        {
            itm.CanEdit = false;
        }

        return new AppClientListResponse(clients);
    }
}

public class AppClientListRequest : IRequest<AppClientListResponse>
{
    public string ClientId { get; set; } = default!;
}

public class AppClientListResponse(IEnumerable<AppClientData> collection) : List<AppClientData>(collection);
