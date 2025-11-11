using ApogeeDev.IdentityProvider.Host.Operations.RequestHandlers;

namespace ApogeeDev.IdentityProvider.Host.Models.ViewModels;


public class ClientListItem
{
    public ClientListItem() { }
    public ClientListItem(AppClientData c)
    {
        ClientId = c.ClientId;
        DisplayName = c.DisplayName;
        ApplicationType = c.ApplicationType;
        ClientType = c.ClientType;
    }
    public string ClientId { get; set; } = default!;
    public string DisplayName { get; set; } = default!;
    public string ApplicationType { get; set; } = default!;
    public string ApplicationTypeCss => GetAppTypeCss();
    public string ClientType { get; set; } = default!;
    public string ClientTypeCss => GetClientTypeCss();

    private string GetAppTypeCss()
    {
        var css = string.Empty;
        switch (ApplicationType.ToUpper())
        {
            case "WEB": css = "text-bg-info"; break;
            case "NATIVE": css = "text-bg-success"; break;
            case "SPA": css = "text-bg-purple-400"; break;
        }
        return css;
    }

    private string GetClientTypeCss()
    {
        var css = string.Empty;
        switch (ClientType.ToUpper())
        {
            case "CONFIDENTIAL": css = "text-bg-warning"; break;
            case "PUBLIC": css = "text-bg-secondary"; break;
        }
        return css;
    }
}

public class ClientListViewModel
{
    public ClientListViewModel()
    {
        Clients = new List<ClientListItem>();
    }
    public List<ClientListItem> Clients { get; set; }

    public ViewMessageState ViewState { get; set; } = default!;
}
