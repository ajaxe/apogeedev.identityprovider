using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class OperationContext
{
    public OperationContext(IOpenIddictMongoDbContext dbContext,
        IOptions<OpenIddictMongoDbOptions> options)
    {
        this.DbContext = dbContext;
        this.Options = options.Value;
    }

    public IOpenIddictMongoDbContext DbContext { get; }
    public OpenIddictMongoDbOptions Options { get; }

    public async Task<IMongoCollection<OpenIddictMongoDbApplication>> GetApplicationsCollectionAsync(
        CancellationToken cancellationToken = default)
    {
        var database = await DbContext.GetDatabaseAsync(cancellationToken);
        return database.GetCollection<OpenIddictMongoDbApplication>(Options.ApplicationsCollectionName);
    }
}
