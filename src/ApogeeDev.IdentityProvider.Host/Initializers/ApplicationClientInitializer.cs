
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;

namespace ApogeeDev.IdentityProvider.Host.Initializers;

public class ApplicationClientInitializer : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly IOptionsMonitor<AppClientOptions> optionsMonitor;
    private readonly ILogger<ApplicationClientInitializer> logger;

    public ApplicationClientInitializer(
        IServiceProvider serviceProvider,
        IOptionsMonitor<AppClientOptions> optionsMonitor,
        ILogger<ApplicationClientInitializer> logger)
    {
        this.serviceProvider = serviceProvider;
        this.optionsMonitor = optionsMonitor;
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Cancellation requested.");
            return;
        }

        logger.LogInformation("Inserting application clients on start-up");
        var currentValue = optionsMonitor.CurrentValue;

        await TryAddOrUpdateAppClient(currentValue);

        optionsMonitor.OnChange(async (current) =>
        {
            logger.LogInformation("Inserting application clients on config change.");
            await TryAddOrUpdateAppClient(current);
        });

        while (!stoppingToken.IsCancellationRequested) await Task.Delay(30000);
    }

    private async Task TryAddOrUpdateAppClient(AppClientOptions currentValue)
    {
        try
        {
            await AddOrUpdateAppClient(currentValue);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error add/update app clients");
        }
    }
    private async Task AddOrUpdateAppClient(AppClientOptions currentValue)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        var clientIds = currentValue.Clients.Select(c => c.ClientId).ToHashSet();
        logger.LogInformation("Removing {@ClientIds}", clientIds);

        await foreach (var entry in manager.ListAsync()
                                            .WhereAwait(async c => !clientIds.Add(await manager.GetClientIdAsync(c)
                                                ?? throw new InvalidOperationException("Invalid client id"))))
        {
            var id = await manager.GetClientIdAsync(entry);
            var displayName = await manager.GetDisplayNameAsync(entry);
            logger.LogInformation("Deleting {@ClientId} {@ClientName}", id, displayName);

            await manager.DeleteAsync(entry);
        }

        foreach (var client in currentValue.Clients)
        {
            logger.LogInformation("Creating {@ClientId} {@ClientName}",
                client.ClientId, client.DisplayName);
            await manager.CreateAsync(GetAppDescriptor(client));
        }
    }

    private OpenIddictApplicationDescriptor GetAppDescriptor(AppClient appClient)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ApplicationType = appClient.ApplicationType,
            DisplayName = appClient.DisplayName,
            ClientId = appClient.ClientId,
            ClientSecret = appClient.ClientSecret,
            ClientType = appClient.ClientType,
        };

        descriptor.RedirectUris.UnionWith(appClient.GetRedirectUris());
        descriptor.Permissions.UnionWith(AppClient.GetPermissionsWithOfflineAccess());
        descriptor.PostLogoutRedirectUris.UnionWith(appClient.GetPostLogoutRedirectUris());

        return descriptor;
    }
}
