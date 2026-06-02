using System.ComponentModel.DataAnnotations;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using OpenIddict.Abstractions;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public static class OAuthFlowTypes
{
    public const string AuthorizationCode = "authorization_code";
    public const string ClientCredentials = "client_credentials";
}

public class AppClientData : IValidatableObject
{
    public string ClientId { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string ApplicationType { get; set; } = default!;
    public string ClientType { get; set; } = default!;
    public string FlowType { get; set; } = OAuthFlowTypes.AuthorizationCode;
    public string[] RedirectUris { get; set; } = default!;
    public string[] PostLogoutRedirectUris { get; set; } = default!;
    public bool CanEdit { get; set; } = true;
    public bool AllowOfflineAccess { get; set; } = false;
    public bool EnablePkce { get; set; } = true;
    public bool SkipAccessTokenEncryption { get; set; } = false; // New property to indicate if access token encryption should be skipped for this client

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(ClientId))
            yield return new ValidationResult("Invalid ClientId", [nameof(ClientId)]);

        if (string.IsNullOrWhiteSpace(DisplayName))
            yield return new ValidationResult("Invalid DisplayName", [nameof(DisplayName)]);

        if (this.FlowType == OAuthFlowTypes.ClientCredentials) yield break; // skip further validation for client credentials flow as some properties are not required

        var appTypes = new[] { "web", "spa", "native" };
        if (appTypes.Contains(ApplicationType?.ToLower()) == false)
            yield return new ValidationResult("Invalid ApplicationType", [nameof(ApplicationType)]);

        var clientTypes = new[] { "confidential", "public" };
        if (clientTypes.Contains(ClientType?.ToLower()) == false)
            yield return new ValidationResult("Invalid ClientType", [nameof(ClientType)]);

        if (RedirectUris == null
            || RedirectUris.Length == 0
            || RedirectUris.Any(s => string.IsNullOrWhiteSpace(s)))
            yield return new ValidationResult("Invalid Redirect uris", [nameof(RedirectUris)]);

        if (PostLogoutRedirectUris == null
            || PostLogoutRedirectUris.Length == 0
            || PostLogoutRedirectUris.Any(s => string.IsNullOrWhiteSpace(s)))
            yield return new ValidationResult("Invalid Redirect uris", [nameof(PostLogoutRedirectUris)]);
    }
}

public class AppClientDataWithSecret : AppClientData
{
    public string ClientSecret { get; set; } = default!;
}

internal static class AppClientDataExtensions
{
    public static (IEnumerable<string>, IEnumerable<string>) MapRequirementPermissions(this AppClientData data)
    {
        var permissions = new List<string>();
        var requirements = new List<string>();

        if (data.FlowType == OAuthFlowTypes.ClientCredentials)
        {
            permissions.AddRange(AppClient.ClientCredentialsPermissions());
        }
        else
        {
            if (data.EnablePkce)
            {
                requirements.Add(OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange);
            }
            permissions.AddRange(
                data.AllowOfflineAccess ?
                AppClient.GetPermissionsWithOfflineAccess() :
                AppClient.DefaultPermissions()
            );
        }

        return (permissions, requirements);
    }
}
