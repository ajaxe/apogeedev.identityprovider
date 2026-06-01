# Adding CLI Support

## Goal
As a developer working on a CLI tool to generate self-signed certificates using the SelfSignedCert C# project, I want to add support for command-line parameters which can customize certificate paramaters.


## Scope

Add support for CLI parameters as follows:
* -o, --out-file-prefix: filename prefix for output file example `server-` results in output server-encryption.pfx and server-signing.pfx
* -nb, --not-before: indicates `not before` validiity date for x509 certificate, a certificate is not valida before this date-time value. C# valid DateTimeOffset value, time component is optional.
  * If time is missing assume current time
  * Default value is current DateTimeOffset.UtcNow
* -na, --not-after: indicates `not after` validiity date for x509 certificate, a certificate is expired after this date-time value. C# valid DateTimeOffset value, time component is optional.
  * the value must be greater than `not before` value.
  * If time is missing assume current time plus 2 years
  * Default value is current DateTimeOffset.UtcNow.AddYears(2)

## Deliverables

* Update CertGen.cs to support new parameters and use them in CreateEncryptionCertificate
* Input validation for not-before and not-after values.
