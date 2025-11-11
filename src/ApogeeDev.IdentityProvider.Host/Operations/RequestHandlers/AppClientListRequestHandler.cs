
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientListRequestHandler : IRequestHandler<AppClientListRequest, AppClientListResponse>
{
    private readonly IOpenIddictMongoDbContext dbContext;
    private readonly OpenIddictMongoDbOptions options;

    public AppClientListRequestHandler(IOpenIddictMongoDbContext dbContext,
        IOptions<OpenIddictMongoDbOptions> options)
    {
        this.dbContext = dbContext;
        this.options = options.Value;
    }

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

        return new AppClientListResponse(clients);
    }
}

public class AppClientListRequest : IRequest<AppClientListResponse>
{
    public string ClientId { get; set; } = default!;
}

public class AppClientData
{
    public string ClientId { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string ApplicationType { get; set; } = default!;
    public string ClientType { get; set; } = default!;
    public string[] RedirectUris { get; set; } = default!;
    public string[] PostLogoutRedirectUris { get; set; } = default!;
}

public class AppClientListResponse : List<AppClientData>
{
    public AppClientListResponse(IEnumerable<AppClientData> collection) : base(collection)
    {
    }
}
