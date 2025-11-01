namespace ApogeeDev.IdentityProvider.Host.Models.ViewModels;

public class MultipleUriViewModel
{
    public string Id { get; set; } = default!;
    public string[] Uris { get; set; } = [];
}
