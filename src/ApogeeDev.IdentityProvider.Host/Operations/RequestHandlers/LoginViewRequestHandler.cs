
using OpenIddict.Abstractions;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class LoginViewRequest : IRequest<LoginViewModel>
{
    public string AuthorizeRedirectUrl { get; set; } = default!;
    public string ClientId { get; set; } = default!;
}

public class LoginViewRequestHandler : IRequestHandler<LoginViewRequest, LoginViewModel>
{
    private readonly ICryptoHelper cryptoHelper;
    private readonly IOpenIddictApplicationManager appManager;

    public LoginViewRequestHandler(ICryptoHelper cryptoHelper,
        IOpenIddictApplicationManager appManager)
    {
        this.cryptoHelper = cryptoHelper;
        this.appManager = appManager;
    }

    public async Task<LoginViewModel> Handle(LoginViewRequest request, CancellationToken cancellationToken)
    {
        var app = await appManager.FindByClientIdAsync(request.ClientId, cancellationToken);
        // get application name & other details for the login view
        return new LoginViewModel
        {
            AppDisplayName = await appManager.GetDisplayNameAsync(app)
                ?? throw new InvalidOperationException($"Invalid Display Name. clientId: {request.ClientId}'"),
            RedirectUrl = cryptoHelper.EncryptAsBase64Url(request.AuthorizeRedirectUrl),
        };
    }
}