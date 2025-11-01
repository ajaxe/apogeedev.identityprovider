using Microsoft.AspNetCore.Mvc.Rendering;

namespace ApogeeDev.IdentityProvider.Host.Models.ViewModels;

public class ModifyClientViewModel
{
    public bool IsEditMode { get; set; }
    public string ClientId { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string ApplicationType { get; set; } = default!;
    public string ClientType { get; set; } = default!;
    public string[] RedirectUris { get; set; } = default!;
    public string[] PostLogoutRedirectUris { get; set; } = default!;

    public MultipleUriViewModel RedirectUrisViewModel => new MultipleUriViewModel
    {
        Id = "redirect-uri",
        Uris = RedirectUris,
    };
    public MultipleUriViewModel PostLogoutRedirectUrisViewModel => new MultipleUriViewModel
    {
        Id = "post-redirect-uris",
        Uris = PostLogoutRedirectUris,
    };


    public List<SelectListItem> AppTypes { get; } = new List<SelectListItem>
    {
        new SelectListItem("Select Application Type", "", selected: true),
        new SelectListItem("Web", "web"),
        new SelectListItem("Native", "native"),
        new SelectListItem("SPA", "spa"),
    };
    public List<SelectListItem> ClientTypes { get; } = new List<SelectListItem>
    {
        new SelectListItem("Select Client Type", "", selected: true),
        new SelectListItem("Public", "public"),
        new SelectListItem("Confidential", "confidential"),
    };
}
