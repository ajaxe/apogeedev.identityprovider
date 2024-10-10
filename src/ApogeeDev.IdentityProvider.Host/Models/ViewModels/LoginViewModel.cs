namespace ApogeeDev.IdentityProvider.Host.Models.ViewModels;

public class LoginViewModel
{
    public string AppDisplayName { get; set; } = default!;
    public string RedirectUrl { get; set; } = default!;
}