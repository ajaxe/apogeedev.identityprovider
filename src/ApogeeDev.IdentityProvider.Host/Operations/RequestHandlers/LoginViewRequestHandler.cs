
namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class LoginViewRequest : IRequest<LoginViewModel>
{
    public string AuthorizeRedirectUrl { get; set; } = default!;
    public string ClientId { get; set; } = default!;
}

public class LoginViewRequestHandler : IRequestHandler<LoginViewRequest, LoginViewModel>
{
    private readonly ICryptoHelper cryptoHelper;

    public LoginViewRequestHandler(ICryptoHelper cryptoHelper)
    {
        this.cryptoHelper = cryptoHelper;
    }

    public Task<LoginViewModel> Handle(LoginViewRequest request, CancellationToken cancellationToken)
    {
        // get application name & other details for the login view
        return Task.FromResult(new LoginViewModel
        {
            AppDisplayName = "My App",
            RedirectUrl = cryptoHelper.EncryptAsBase64Url(request.AuthorizeRedirectUrl),
        });
    }
}