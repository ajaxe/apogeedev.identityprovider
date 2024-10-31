namespace ApogeeDev.IdentityProvider.Host.Models.Configuration;

public static class CustomClaimTypes
{
    public static class GitHub
    {
        public const string AvatarUrl = "avatar_url";
        public const string Login = "login";
    }
    /// <summary>
    /// Custom claims defined by our idp server
    /// </summary> <summary>
    ///
    /// </summary>
    public static class IdpServer
    {
        public const string IdP = "private:idp";
    }
    public static class Common
    {
        public const string IdentityProvider = "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider";
    }
    public static class Google
    {
        public const string Picture = "picture";
    }
}