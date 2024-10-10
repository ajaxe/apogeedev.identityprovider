
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

        await AddOrUpdateAppClient(currentValue);

        optionsMonitor.OnChange(async (current) =>
        {
            logger.LogInformation("Inserting application clients on config change.");
            await AddOrUpdateAppClient(current);
        });

        while (!stoppingToken.IsCancellationRequested) await Task.Delay(5000);
    }

    private async Task AddOrUpdateAppClient(AppClientOptions currentValue)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

        await foreach (var entry in manager.ListAsync())
        {
            await manager.DeleteAsync(entry);
        }
        foreach (var client in currentValue.Clients)
        {
            await manager.CreateAsync(GetAppDescriptor(client));
        }
    }

    private OpenIddictApplicationDescriptor GetAppDescriptor(AppClient appClient)
    {
        var descriptor = new OpenIddictApplicationDescriptor
        {
            ApplicationType = appClient.ApplicationType,
            ClientId = appClient.ClientId,
            ClientSecret = appClient.ClientSecret,
            ClientType = appClient.ClientType,
        };

        descriptor.RedirectUris.UnionWith(appClient.GetUris());
        descriptor.Permissions.UnionWith(appClient.GetPermissions());

        return descriptor;
    }
}