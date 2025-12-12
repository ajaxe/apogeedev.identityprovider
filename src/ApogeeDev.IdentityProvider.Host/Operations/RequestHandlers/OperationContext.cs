using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class OperationContext(IOpenIddictMongoDbContext dbContext,
    IOptions<OpenIddictMongoDbOptions> options,
    IOptionsSnapshot<AppClientOptions> appClientOptions)
{
    public IOpenIddictMongoDbContext DbContext { get; } = dbContext;
    public OpenIddictMongoDbOptions Options { get; } = options.Value;
    public AppClientOptions AppClientOptions { get; } = appClientOptions.Value;

    public async Task<IMongoCollection<OpenIddictMongoDbApplication>> GetApplicationsCollectionAsync(
        CancellationToken cancellationToken = default)
    {
        var database = await DbContext.GetDatabaseAsync(cancellationToken);
        return database.GetCollection<OpenIddictMongoDbApplication>(Options.ApplicationsCollectionName);
    }
}
