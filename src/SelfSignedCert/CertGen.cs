using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SelfSignedCert;

public static class CertGen
{
    public const string CertName = "Apogee-Dev Identity Server";
    public static void CreateEncryptionCertificate()
    {
        using var algorithm = RSA.Create(keySizeInBits: 2048);

        var subject = new X500DistinguishedName($"CN={CertName} Encryption Certificate");
        var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, critical: true));

        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

        DeleteExisting("server-encryption-certificate.pfx");

        File.WriteAllBytes("server-encryption-certificate.pfx", certificate.Export(X509ContentType.Pfx, string.Empty));
    }

    public static void CreateSigningCertificate()
    {
        using var algorithm = RSA.Create(keySizeInBits: 2048);

        var subject = new X500DistinguishedName($"CN={CertName} Signing Certificate");
        var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));

        var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

        DeleteExisting("server-signing-certificate.pfx");

        File.WriteAllBytes("server-signing-certificate.pfx", certificate.Export(X509ContentType.Pfx, string.Empty));
    }

    private static void DeleteExisting(string name)
    {
        if (File.Exists(name))
        {
            File.Delete(name);
        }
    }
}