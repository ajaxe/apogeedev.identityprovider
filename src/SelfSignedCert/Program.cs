using SelfSignedCert;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

CertGen.CreateEncryptionCertificate();

CertGen.CreateSigningCertificate();