using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using OpenIddict.Abstractions;
using OpenIddict.Client;
using OpenIddict.Client.WebIntegration;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ApogeeDev.IdentityProvider.Host.Helpers.Authentication;

public static class AuthClientExtension
{
    public static void ConfigureOpenIdDictClient(this OpenIddictClientBuilder options,
        IConfiguration Configuration)
    {
        var webProviders = new OAuthWebProviderOptions();
        Configuration.GetSection(OAuthWebProviderOptions.SectionName)
            .Bind(webProviders);

        options.RemoveEventHandler(OpenIddictClientHandlers.ValidateIssuerParameter.Descriptor);

        options.AddEventHandler<OpenIddictClientEvents.ProcessAuthenticationContext>(
            builder => DisableIssuerValidation(builder));

        options.AllowAuthorizationCodeFlow()
            .AllowRefreshTokenFlow()
            .AddDevelopmentEncryptionCertificate()
            .AddDevelopmentSigningCertificate()
            .UseAspNetCore()
            .EnableStatusCodePagesIntegration()
            .EnableRedirectionEndpointPassthrough();

        options.UseSystemNetHttp()
            .SetProductInformation(typeof(Startup).Assembly);

        options.UseWebProviders()
            .AddGitHub(opts =>
            {
                opts.SetClientId(webProviders.Github.ClientId)
                    .SetClientSecret(webProviders.Github.ClientSecret)
                    .SetRedirectUri(webProviders.Github.RedirectUri)
                    .AddScopes(webProviders.Github.Scopes);
            })
            .AddGoogle(opts =>
            {
                opts.SetClientId(webProviders.Google.ClientId)
                    .SetClientSecret(webProviders.Google.ClientSecret)
                    .SetRedirectUri(webProviders.Google.RedirectUri)
                    .AddScopes(webProviders.Google.Scopes);
            });
    }

    private static void DisableIssuerValidation(
        OpenIddictClientHandlerDescriptor.Builder<OpenIddictClientEvents.ProcessAuthenticationContext> builder)
    {
        builder.Import(OpenIddictClientHandlers.ValidateIssuerParameter.Descriptor);
        builder.SetType(OpenIddictClientHandlerType.Custom);

        builder.UseInlineHandler(context =>
        {
            // To help mitigate mix-up attacks, the identity of the issuer can be returned by
            // authorization servers that support it as part of the "iss" parameter, which
            // allows comparing it to the issuer in the state token. Depending on the selected
            // response_type, the same information could be retrieved from the identity token
            // that is expected to contain an "iss" claim containing the issuer identity.
            //
            // This handler eagerly validates the "iss" parameter if the authorization server
            // is known to support it (and automatically rejects the request if it doesn't).
            // Validation based on the identity token is performed later in the pipeline.
            //
            // See https://datatracker.ietf.org/doc/html/draft-ietf-oauth-security-topics-19#section-4.4
            // for more information.
            var issuer = (string?)context.Request[Parameters.Iss];

            if (context.Configuration.AuthorizationResponseIssParameterSupported is true ||
                (context.Registration.ProviderType is OpenIddictClientWebIntegrationConstants.ProviderTypes.Google &&
                !string.IsNullOrEmpty(issuer)))
            {
                // Reject authorization responses that don't contain the "iss" parameter
                // if the server configuration indicates this parameter should be present.
                if (string.IsNullOrEmpty(issuer))
                {
                    context.Reject(
                        error: Errors.InvalidRequest,
                        description: OpenIddictResources.FormatID2029(Parameters.Iss),
                        uri: OpenIddictResources.FormatID8000(OpenIddictResources.ID2029));

                    return ValueTask.CompletedTask;
                }

                // If the two values don't match, this may indicate a mix-up attack attempt.
                if (!Uri.TryCreate(issuer, UriKind.Absolute, out Uri? uri) || uri != context.Registration.Issuer)
                {
                    context.Reject(
                        error: Errors.InvalidRequest,
                        description: OpenIddictResources.FormatID2119(Parameters.Iss),
                        uri: OpenIddictResources.FormatID8000(OpenIddictResources.ID2119));

                    return ValueTask.CompletedTask;
                }
            }

            // Reject authorization responses containing an "iss" parameter if the configuration
            // doesn't indicate this parameter is supported, as recommended by the specification.
            // See https://datatracker.ietf.org/doc/html/draft-ietf-oauth-iss-auth-resp-05#section-2.4
            // for more information.
            else if (context.Registration.ProviderType is not OpenIddictClientWebIntegrationConstants.ProviderTypes.Google &&
                !string.IsNullOrEmpty(issuer))
            {
                context.Reject(
                    error: Errors.InvalidRequest,
                    description: OpenIddictResources.FormatID2120(Parameters.Iss, Metadata.AuthorizationResponseIssParameterSupported),
                    uri: OpenIddictResources.FormatID8000(OpenIddictResources.ID2120));

                return ValueTask.CompletedTask;
            }

            return ValueTask.CompletedTask;
        });
    }
}
