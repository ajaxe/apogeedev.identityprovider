using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

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

        // Fetch data into an anonymous type first to avoid MongoDB projection errors
        // when fields (like Requirements) are missing in the document.
        var result = await query.Select(app => new
        {
            app.DisplayName,
            app.ClientId,
            app.ApplicationType,
            app.ClientType,
            app.RedirectUris,
            app.PostLogoutRedirectUris,
            app.Permissions,
            app.Requirements
        }).ToListAsync(cancellationToken);

        var clients = result.Select(app => new AppClientData
        {
            DisplayName = app.DisplayName ?? string.Empty,
            ClientId = app.ClientId ?? string.Empty,
            ApplicationType = app.ApplicationType ?? string.Empty,
            ClientType = app.ClientType ?? string.Empty,
            RedirectUris = app.RedirectUris == null ? [] : [.. app.RedirectUris],
            PostLogoutRedirectUris = app.PostLogoutRedirectUris == null ? [] : [.. app.PostLogoutRedirectUris],
            AllowOfflineAccess = app.Permissions != null && app.Permissions.Contains(AppClient.OfflineAccessScope),
            EnablePkce = app.Requirements != null && app.Requirements.Contains(Requirements.Features.ProofKeyForCodeExchange),
        }).ToList();

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
