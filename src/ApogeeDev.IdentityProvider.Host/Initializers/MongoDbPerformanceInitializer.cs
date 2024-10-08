
using ApogeeDev.IdentityProvider.Host.Models.DatabaseModels;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Initializers;

internal class MongoDbPerformanceInitializer : BackgroundService
{
    private readonly IMongoDatabase database;
    private readonly OpenIddictMongoDbOptions opendIddictOptions;
    private readonly ILogger<MongoDbPerformanceInitializer> logger;

    public MongoDbPerformanceInitializer(IMongoDatabase database,
        IOptionsMonitor<OpenIddictMongoDbOptions> opendIddictOptions,
        ILogger<MongoDbPerformanceInitializer> logger)
    {
        this.database = database;
        this.opendIddictOptions = opendIddictOptions.CurrentValue;
        this.logger = logger;
    }

    private ExpressionFilterDefinition<AppDbSettings> GlobalSettingFilter =>
        new ExpressionFilterDefinition<AppDbSettings>(f => f.SettingType == DbSettingType.Global);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Performance indexing code start.");

        if (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Cancellation requested.");
            return;
        }

        logger.LogInformation("Performance indexing code run: delayed.");
        await Task.Delay(5000);

        try
        {
            await Initialize();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during initialization: {message}", ex.Message);
        }
    }

    private async Task Initialize()
    {
        if (!await ShouldRunScript())
        {
            logger.LogInformation("Performance indexing code is already run.");
            return;
        }

        await AddApplicationIndexes();

        await AddAuthorizationIndexes();

        var scopes = database.GetCollection<OpenIddictMongoDbScope>(
            opendIddictOptions.ScopesCollectionName);

        await scopes.Indexes.CreateOneAsync(new CreateIndexModel<OpenIddictMongoDbScope>(
            Builders<OpenIddictMongoDbScope>.IndexKeys.Ascending(scope => scope.Name),
            new CreateIndexOptions
            {
                Unique = true
            }));

        await AddTokenIndexes();

        await SetPerformanceIndexCreated();

        logger.LogInformation("Performance indexing code finished.");
    }

    private async Task AddTokenIndexes()
    {
        var tokens = database.GetCollection<OpenIddictMongoDbToken>(opendIddictOptions.TokensCollectionName);

        await tokens.Indexes.CreateManyAsync(
        [
            new CreateIndexModel<OpenIddictMongoDbToken>(
        Builders<OpenIddictMongoDbToken>.IndexKeys.Ascending(token => token.ReferenceId),
        new CreateIndexOptions<OpenIddictMongoDbToken>
        {
            // Note: partial filter expressions are not supported on Azure Cosmos DB.
            // As a workaround, the expression and the unique constraint can be removed.
            PartialFilterExpression = Builders<OpenIddictMongoDbToken>.Filter.Exists(token => token.ReferenceId),
            Unique = true
        }),

        new CreateIndexModel<OpenIddictMongoDbToken>(
            Builders<OpenIddictMongoDbToken>.IndexKeys.Ascending(token => token.AuthorizationId),
            new CreateIndexOptions<OpenIddictMongoDbToken>()
            {
                PartialFilterExpression =
                    Builders<OpenIddictMongoDbToken>.Filter.Exists(token => token.AuthorizationId),
            }),

        new CreateIndexModel<OpenIddictMongoDbToken>(
            Builders<OpenIddictMongoDbToken>.IndexKeys
                .Ascending(token => token.ApplicationId)
                .Ascending(token => token.Status)
                .Ascending(token => token.Subject)
                .Ascending(token => token.Type),
            new CreateIndexOptions
            {
                Background = true
            })
        ]);
    }

    private async Task SetPerformanceIndexCreated(bool value = true)
    {
        var existing = (await database.ListCollectionNames()
            .ToListAsync()).FirstOrDefault(n => AppDbSettings.CollectionName.Equals(n));

        if (existing is null)
        {
            await database.CreateCollectionAsync(AppDbSettings.CollectionName);
            logger.LogInformation("Creating settings collection");
        }

        var collection = database.GetCollection<AppDbSettings>(AppDbSettings.CollectionName);

        if (await collection.Find(GlobalSettingFilter).FirstOrDefaultAsync() is null)
        {
            await collection.InsertOneAsync(new AppDbSettings
            {
                PerformanceIndexCreated = value,
            });
        }
        else
        {
            var update = Builders<AppDbSettings>.Update
                    .Set(f => f.PerformanceIndexCreated, value);
            await database.GetCollection<AppDbSettings>(AppDbSettings.CollectionName)
                .UpdateOneAsync(GlobalSettingFilter, update);
        }

        logger.LogInformation("Performance Indexes created: {value}", value);
    }

    private async Task<bool> ShouldRunScript()
    {
        AppDbSettings? settings = await database.GetCollection<AppDbSettings>(AppDbSettings.CollectionName)
            .Find(GlobalSettingFilter)
            .FirstOrDefaultAsync();

        return !settings?.PerformanceIndexCreated.GetValueOrDefault(true) ?? true;
    }

    private async Task AddAuthorizationIndexes()
    {
        var authorizations = database.GetCollection<OpenIddictMongoDbAuthorization>(
            opendIddictOptions.AuthorizationsCollectionName);

        await authorizations.Indexes.CreateOneAsync(
        new CreateIndexModel<OpenIddictMongoDbAuthorization>(
            Builders<OpenIddictMongoDbAuthorization>.IndexKeys
                .Ascending(authorization => authorization.ApplicationId)
                .Ascending(authorization => authorization.Scopes)
                .Ascending(authorization => authorization.Status)
                .Ascending(authorization => authorization.Subject)
                .Ascending(authorization => authorization.Type),
            new CreateIndexOptions
            {
                Background = true
            }));
    }

    private async Task AddApplicationIndexes()
    {
        var applications = database.GetCollection<OpenIddictMongoDbApplication>(
            opendIddictOptions.ApplicationsCollectionName);

        await applications.Indexes.CreateManyAsync(
        [
            new CreateIndexModel<OpenIddictMongoDbApplication>(
            Builders<OpenIddictMongoDbApplication>.IndexKeys.Ascending(application => application.ClientId),
            new CreateIndexOptions
            {
                Unique = true
            }),

            new CreateIndexModel<OpenIddictMongoDbApplication>(
            Builders<OpenIddictMongoDbApplication>.IndexKeys.Ascending(application => application.PostLogoutRedirectUris),
            new CreateIndexOptions
            {
                Background = true
            }),

            new CreateIndexModel<OpenIddictMongoDbApplication>(
            Builders<OpenIddictMongoDbApplication>.IndexKeys.Ascending(application => application.RedirectUris),
            new CreateIndexOptions
            {
                Background = true
            })
        ]);
    }
}