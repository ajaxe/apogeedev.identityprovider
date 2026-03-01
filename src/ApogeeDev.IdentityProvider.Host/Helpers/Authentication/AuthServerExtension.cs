using System.Security.Cryptography.X509Certificates;
using OpenIddict.Abstractions;

namespace ApogeeDev.IdentityProvider.Host.Helpers.Authentication;

public static class AuthServerExtension
{
    public static void ConfigureOpenIdDictServer(this OpenIddictServerBuilder o,
        string AppPathPrefix,
        IConfiguration Configuration)
    {
        o.RegisterScopes(
            OpenIddictConstants.Scopes.Address,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Phone,
            OpenIddictConstants.Scopes.Roles,
            OpenIddictConstants.Scopes.OfflineAccess);

        var encryptionCert = X509CertificateLoader.LoadPkcs12(File.ReadAllBytes(Configuration["AppOptions:EncryptionCert"]!), null);
        var signingCert = X509CertificateLoader.LoadPkcs12(File.ReadAllBytes(Configuration["AppOptions:SigningCert"]!), null);

        o.SetAuthorizationEndpointUris($"{AppPathPrefix}/connect/authorize")
            .SetTokenEndpointUris($"{AppPathPrefix}/connect/token")
            .SetEndSessionEndpointUris($"{AppPathPrefix}/connect/logout")
            .SetUserInfoEndpointUris($"{AppPathPrefix}/connect/userinfo")
            .AllowAuthorizationCodeFlow()
            .AllowRefreshTokenFlow()
            // .RequireProofKeyForCodeExchange() // Enable PKCE per client configuration
            .AddEncryptionCertificate(encryptionCert)
            .AddSigningCertificate(signingCert)
            .UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableEndSessionEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableUserInfoEndpointPassthrough()
            .DisableTransportSecurityRequirement();
    }
}
