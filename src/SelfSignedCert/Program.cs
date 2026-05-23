using SelfSignedCert;
using System.CommandLine;

var rootCommand = new RootCommand("Self-signed certificate generator");

var outFilePrefixOption = new Option<string>("--out-file-prefix", "Prefix for the output files");
outFilePrefixOption.AddAlias("-o");

var notBeforeOption = new Option<DateTimeOffset?>("--not-before", "The not-before date");
notBeforeOption.AddAlias("-nb");

var notAfterOption = new Option<DateTimeOffset?>("--not-after", "The not-after date");
notAfterOption.AddAlias("-na");

rootCommand.AddOption(outFilePrefixOption);
rootCommand.AddOption(notBeforeOption);
rootCommand.AddOption(notAfterOption);

rootCommand.SetHandler((string? outFilePrefix, DateTimeOffset? notBefore, DateTimeOffset? notAfter) =>
{
    string prefix = outFilePrefix ?? "server-";
    DateTimeOffset nb = notBefore ?? DateTimeOffset.UtcNow;
    DateTimeOffset na = notAfter ?? nb.AddYears(2);

    try
    {
        CertGen.CreateEncryptionCertificate(prefix, nb, na);
        CertGen.CreateSigningCertificate(prefix, nb, na);
    }
    catch (ArgumentException ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        Environment.Exit(1);
    }
}, outFilePrefixOption, notBeforeOption, notAfterOption);

await rootCommand.InvokeAsync(args);

