namespace ApogeeDev.IdentityProvider.Host.Models.ViewModels;

public class ViewMessageState
{
    public const string TempDataKey = "ViewState";
    public bool IsError { get; set; }
    public string Message { get; set; } = default!;

    public static ViewMessageState Error(string message)
    {
        return new ViewMessageState
        {
            IsError = true,
            Message = message,
        };
    }
}
