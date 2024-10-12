namespace ApogeeDev.IdentityProvider.Host.Models.Configuration;

public static class CustomClaimTypes
{
    public static class GitHub
    {
        public static string AvatarUrl { get; } = "avatar_url";
        public static string IdentityProvider { get; } = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
        public static string Login { get; } = "login";
    }
    /// <summary>
    /// Custom claims defined by our idp server
    /// </summary> <summary>
    ///
    /// </summary>
    public static class IdpServer
    {
        public static string IdP { get; } = "private:idp";
    }
}