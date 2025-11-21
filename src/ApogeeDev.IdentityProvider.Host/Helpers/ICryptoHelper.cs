namespace ApogeeDev.IdentityProvider.Host.Helpers;
public interface ICryptoHelper
{
    string EncryptAsBase64Url(string plainText);
    string DecryptAsBase64Url(string cupherText);

    byte[] GenerateRandom(int? byteCount = null);
}
