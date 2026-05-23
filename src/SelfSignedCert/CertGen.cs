using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SelfSignedCert;

public static class CertGen
{
    public const string CertName = "Apogee-Dev Identity Server";
    public const int DefaultKeySizeInBits = 2048;
    public static void CreateEncryptionCertificate(string outFilePrefix = "server-", DateTimeOffset? notBefore = null, DateTimeOffset? notAfter = null)
    {
        notBefore ??= DateTimeOffset.UtcNow;
        notAfter ??= notBefore.Value.AddYears(2);

        if (notAfter <= notBefore)
        {
            throw new ArgumentException("not-after must be greater than not-before");
        }

        var certificate = GenerateCertificate($"CN={CertName} Encryption Certificate", X509KeyUsageFlags.KeyEncipherment,
            notBefore.Value, notAfter.Value);

        DeleteExisting($"{outFilePrefix}encryption.pfx");

        File.WriteAllBytes($"{outFilePrefix}encryption.pfx", certificate.Export(X509ContentType.Pfx, string.Empty));
    }

    public static void CreateSigningCertificate(string outFilePrefix = "server-", DateTimeOffset? notBefore = null, DateTimeOffset? notAfter = null)
    {
        notBefore ??= DateTimeOffset.UtcNow;
        notAfter ??= notBefore.Value.AddYears(2);

        if (notAfter <= notBefore)
        {
            throw new ArgumentException("not-after must be greater than not-before");
        }

        var certificate = GenerateCertificate($"CN={CertName} Signing Certificate", X509KeyUsageFlags.DigitalSignature,
            notBefore.Value, notAfter.Value);

        DeleteExisting($"{outFilePrefix}signing.pfx");

        File.WriteAllBytes($"{outFilePrefix}signing.pfx", certificate.Export(X509ContentType.Pfx, string.Empty));
    }

    private static X509Certificate2 GenerateCertificate(string commonName,
        X509KeyUsageFlags keyUsageFlags, DateTimeOffset notBefore, DateTimeOffset notAfter)
    {
        using var algorithm = RSA.Create(keySizeInBits: DefaultKeySizeInBits);

        var subject = new X500DistinguishedName($"CN={commonName}");
        var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(keyUsageFlags, critical: true));

        var certificate = request.CreateSelfSigned(notBefore, notAfter);

        return certificate;
    }

    private static void DeleteExisting(string name)
    {
        if (File.Exists(name))
        {
            File.Delete(name);
        }
    }
}
