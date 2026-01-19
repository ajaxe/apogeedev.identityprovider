using MongoDB.Driver;
using OpenIddict.MongoDb.Models;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientDeleteRequest : IRequest<AppClientDeleteResponse>
{
    public string ClientId { get; set; } = default!;
}

public class AppClientDeleteResponse { }

public class AppClientDeleteRequestHandler(OperationContext opContext, ILogger<AppClientDeleteRequestHandler> logger) : IRequestHandler<AppClientDeleteRequest, AppClientDeleteResponse>
{
    public async Task<AppClientDeleteResponse> Handle(AppClientDeleteRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        ArgumentException.ThrowIfNullOrWhiteSpace(request.ClientId);

        opContext.AppClientOptions.ThrowIfStaticClient(request.ClientId);

        var existing = await opContext.ApplicationManager.FindByClientIdAsync(request.ClientId, cancellationToken);

        if (existing is null)
        {
            if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("Application not found, {ClientId}", request.ClientId);

            return new AppClientDeleteResponse();
        }

        await opContext.ApplicationManager.DeleteAsync(existing, cancellationToken);

        return new AppClientDeleteResponse();
    }
}
