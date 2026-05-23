using System.Security.Cryptography.X509Certificates;
using OpenIddict.Abstractions;

namespace ApogeeDev.IdentityProvider.Host.Helpers.Authentication;

public static class AuthServerExtension
{
    public static void ConfigureOpenIdDictServer(this OpenIddictServerBuilder o,
        string AppPathPrefix,
        IConfiguration Configuration, ILogger logger)
    {
        o.RegisterScopes(
            OpenIddictConstants.Scopes.Address,
            OpenIddictConstants.Scopes.Profile,
            OpenIddictConstants.Scopes.Email,
            OpenIddictConstants.Scopes.Phone,
            OpenIddictConstants.Scopes.Roles,
            OpenIddictConstants.Scopes.OfflineAccess);

        var encryptionCerts = LoadEncryptionCerts(Configuration, logger);
        var signingCerts = LoadSigningCerts(Configuration, logger);

        o.SetAuthorizationEndpointUris($"{AppPathPrefix}/connect/authorize")
            .SetTokenEndpointUris($"{AppPathPrefix}/connect/token")
            .SetEndSessionEndpointUris($"{AppPathPrefix}/connect/logout")
            .SetUserInfoEndpointUris($"{AppPathPrefix}/connect/userinfo")
            .AllowAuthorizationCodeFlow()
            .AllowRefreshTokenFlow()
            // .RequireProofKeyForCodeExchange() // Enable PKCE per client configuration
            .AddEncryptionCertificates(encryptionCerts)
            .AddSigningCertificates(signingCerts)
            .UseAspNetCore()
            .EnableAuthorizationEndpointPassthrough()
            .EnableEndSessionEndpointPassthrough()
            .EnableTokenEndpointPassthrough()
            .EnableUserInfoEndpointPassthrough()
            .DisableTransportSecurityRequirement();
    }

    private static IEnumerable<X509Certificate2> LoadEncryptionCerts(IConfiguration configuration, ILogger logger)
    {
        yield return X509CertificateLoader.LoadPkcs12(File.ReadAllBytes(configuration["AppOptions:EncryptionCert"]!), null);

        var suffix = configuration["AppOptions:EncryptionCertSuffix"];

        foreach (var file in LoadCertsUsingsuffix(configuration, suffix, logger))
        {
            yield return file;
        }
    }
    private static IEnumerable<X509Certificate2> LoadSigningCerts(IConfiguration configuration, ILogger logger)
    {
        yield return X509CertificateLoader.LoadPkcs12(File.ReadAllBytes(configuration["AppOptions:SigningCert"]!), null);

        var suffix = configuration["AppOptions:SigningCertSuffix"];

        foreach (var file in LoadCertsUsingsuffix(configuration, suffix, logger))
        {
            yield return file;
        }
    }
    private static IEnumerable<X509Certificate2> LoadCertsUsingsuffix(IConfiguration configuration, string? suffix, ILogger logger)
    {
        var certFolder = configuration["AppOptions:CertFolder"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(certFolder) || string.IsNullOrWhiteSpace(suffix))
        {
            logger.LogInformation("Certificate {folder} or {suffix} not configured, skipping additional cert loading.",
                certFolder, suffix);
            yield break;
        }

        var files = Directory.GetFiles(certFolder, suffix);
        foreach (var file in files)
        {
            logger.LogInformation("Loading additional certificate from file: {file}", file);
            yield return X509CertificateLoader.LoadPkcs12(File.ReadAllBytes(file), null);
        }
    }
}
