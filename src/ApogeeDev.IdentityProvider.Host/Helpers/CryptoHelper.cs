using System.Security.Cryptography;
using ApogeeDev.IdentityProvider.Host.Models.Configuration;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace ApogeeDev.IdentityProvider.Host.Helpers;

public class CryptoHelper : ICryptoHelper
{
    private const int SaltSize = 16; // Size of salt in bytes
    private static readonly int KeySize = 32; // Size of AES key (256 bits)
    private static readonly int IvSize = 16; // Size of IV (128 bits)
    private static readonly int KeyStretchIterations = 10000;
    private readonly AppOptions options;

    public CryptoHelper(IOptionsMonitor<AppOptions> options)
    {
        this.options = options.CurrentValue;
    }

    public string EncryptAsBase64Url(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText)) return plainText;

        byte[] salt = GenerateRandom();
        var cipherBytes = Encrypt(plainText, salt);

        return WebEncoders.Base64UrlEncode(salt.Concat(cipherBytes).ToArray());
    }
    public string DecryptAsBase64Url(string cipherTextBase64Url)
    {
        if (string.IsNullOrWhiteSpace(cipherTextBase64Url)) return cipherTextBase64Url;

        var combinedBytes = WebEncoders.Base64UrlDecode(cipherTextBase64Url);
        var salt = combinedBytes.Take(SaltSize).ToArray();
        var encryptedBytes = combinedBytes.Skip(SaltSize).ToArray();

        return Decrypt(encryptedBytes, salt);
    }

    public byte[] Encrypt(string plaintext, byte[] salt)
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = KeySize * 8; // Key size in bits
            aes.GenerateIV(); // Generate a random IV
            Rfc2898DeriveBytes key = GetKeyStretchData(salt);

            aes.Key = key.GetBytes(KeySize);
            aes.IV = aes.IV;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                // Prepend the IV to the encrypted data
                ms.Write(aes.IV, 0, aes.IV.Length);
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plaintext);
                    }
                }

                return ms.ToArray();
            }
        }
    }

    private Rfc2898DeriveBytes GetKeyStretchData(byte[] salt)
    {
        // Derive the key using PBKDF2
        return new Rfc2898DeriveBytes(options.EncryptionKeyPassword, salt, KeyStretchIterations,
            HashAlgorithmName.SHA256);
    }

    public string Decrypt(byte[] encryptedBytes, byte[] salt)
    {
        using (var aes = Aes.Create())
        {
            aes.KeySize = KeySize * 8;

            // Extract the IV from the beginning of the encrypted data
            var iv = new byte[IvSize];
            Array.Copy(encryptedBytes, 0, iv, 0, IvSize);
            var cipherText = new byte[encryptedBytes.Length - IvSize];
            Array.Copy(encryptedBytes, IvSize, cipherText, 0, cipherText.Length);

            // Derive the key using PBKDF2
            var key = GetKeyStretchData(salt);

            aes.Key = key.GetBytes(KeySize);
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(cipherText))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }

    public byte[] GenerateRandom(int? byteCount = null)
    {
        byteCount = byteCount ?? SaltSize;
        return RandomNumberGenerator.GetBytes(byteCount.Value);
    }
}
