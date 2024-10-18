using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ApogeeDev.IdentityProvider.Host.Helpers;

public class DbConnectionHealthCheck : IHealthCheck
{
    private readonly IOptionsMonitor<AppOptions> options;
    private readonly ILogger<DbConnectionHealthCheck> logger;
    private readonly IMongoClient dbClient;

    public DbConnectionHealthCheck(IMongoClient dbClient, IOptionsMonitor<AppOptions> options,
        ILogger<DbConnectionHealthCheck> logger)
    {
        this.dbClient = dbClient;
        this.options = options;
        this.logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var list = await dbClient.GetDatabase(options.CurrentValue.DatabaseName)
                .ListCollectionNamesAsync();
            var first = await list.FirstOrDefaultAsync();
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Db health check error");
            return HealthCheckResult.Unhealthy();
        }
    }
}