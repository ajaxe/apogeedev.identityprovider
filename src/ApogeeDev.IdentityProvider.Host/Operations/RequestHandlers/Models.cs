using System.ComponentModel.DataAnnotations;

namespace ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

public class AppClientData : IValidatableObject
{
    public string ClientId { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string ApplicationType { get; set; } = default!;
    public string ClientType { get; set; } = default!;
    public string[] RedirectUris { get; set; } = default!;
    public string[] PostLogoutRedirectUris { get; set; } = default!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(ClientId))
            yield return new ValidationResult("Invalid ClientId", [nameof(ClientId)]);

        if (string.IsNullOrWhiteSpace(DisplayName))
            yield return new ValidationResult("Invalid DisplayName", [nameof(DisplayName)]);

        var appTypes = new[] { "web", "spa", "native" };
        if (appTypes.Contains(ApplicationType?.ToLower()) == false)
            yield return new ValidationResult("Invalid ApplicationType", [nameof(ApplicationType)]);

        var clientTypes = new[] { "confidential", "public" };
        if (clientTypes.Contains(ClientType?.ToLower()) == false)
            yield return new ValidationResult("Invalid ClientType", [nameof(ClientType)]);

        if (RedirectUris == null || RedirectUris.Any(s => string.IsNullOrWhiteSpace(s)))
            yield return new ValidationResult("Invalid Redirect uris", [nameof(RedirectUris)]);

        if (PostLogoutRedirectUris == null || PostLogoutRedirectUris.Any(s => string.IsNullOrWhiteSpace(s)))
            yield return new ValidationResult("Invalid Redirect uris", [nameof(PostLogoutRedirectUris)]);
    }
}

public class AppClientDataWithSecret : AppClientData
{
    public string ClientSecret { get; set; } = default!;
}
