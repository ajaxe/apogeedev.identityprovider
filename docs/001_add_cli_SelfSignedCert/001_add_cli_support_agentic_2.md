# Phase 2: Implement CLI Parsing in Program.cs

## Objective
Update `Program.cs` to parse command-line arguments and pass them to the methods in the existing static class `CertGen.cs`.

## Steps
1. Implement logic in `src/SelfSignedCert/Program.cs` to parse `args`.
2. Support `-o` or `--out-file-prefix`: string value.
3. Support `-nb` or `--not-before`: parse as `DateTimeOffset`.
   * **Rule**: If the provided string only contains a date (time is missing), set the time component to the current time.
4. Support `-na` or `--not-after`: parse as `DateTimeOffset`.
   * **Rule**: If the provided string only contains a date (time is missing), set the time component to the current time plus 2 years.
5. If `-nb` is omitted entirely, default to `DateTimeOffset.UtcNow`.
6. If `-na` is omitted entirely, default to the evaluated `notBefore` value plus 2 years.
7. Call `CreateEncryptionCertificate` and `CreateSigningCertificate` on the existing static class `CertGen.cs` with the parsed values.
8. Catch any `ArgumentException` (like the one added in Phase 1) to print a friendly error message and exit with a non-zero code.
