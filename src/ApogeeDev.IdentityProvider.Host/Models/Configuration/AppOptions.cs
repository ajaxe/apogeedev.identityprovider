namespace ApogeeDev.IdentityProvider.Host.Models.Configuration;

public class AppOptions
{
    public const string SectionName = nameof(AppOptions);
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string MongoDbConnection { get; set; } = default!;
    public string DatabaseName { get; set; } = default!;
    public string EncryptionKeyPassword { get; set; } = default!;
    public string[] AppManagerEmails { get; set; } = default!;
}
