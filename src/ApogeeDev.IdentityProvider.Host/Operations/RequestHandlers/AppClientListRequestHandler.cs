
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientListRequestHandler : IRequestHandler<AppClientListRequest, ClientListViewModel>
{
    private readonly IOpenIddictMongoDbContext dbContext;
    private readonly OpenIddictMongoDbOptions options;

    public AppClientListRequestHandler(IOpenIddictMongoDbContext dbContext,
        IOptions<OpenIddictMongoDbOptions> options)
    {
        this.dbContext = dbContext;
        this.options = options.Value;
    }

    public async Task<ClientListViewModel> Handle(AppClientListRequest request,
        CancellationToken cancellationToken)
    {
        var database = await dbContext.GetDatabaseAsync(cancellationToken);
        var collection = database.GetCollection<OpenIddictMongoDbApplication>(options.ApplicationsCollectionName);

        var query = collection.AsQueryable().OrderBy(x => (x.DisplayName ?? string.Empty).ToUpper());

        var clients = await query
            .Select(app => new ClientListItem
            {
                DisplayName = app.DisplayName ?? string.Empty,
                ClientId = app.ClientId ?? string.Empty,
                ApplicationType = app.ApplicationType ?? string.Empty,
                ClientType = app.ClientType ?? string.Empty,
            })
            .ToListAsync(cancellationToken);

        return new ClientListViewModel
        {
            Clients = clients,
        };
    }
}

public class AppClientListRequest : IRequest<ClientListViewModel>
{
}
