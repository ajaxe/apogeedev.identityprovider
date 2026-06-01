# Phase 1: Update CertGen logic

## Objective
Update the existing static class `CertGen.cs` to support the new file prefix parameter and implement input validation for the dates.

## Steps
1. Modify `CreateEncryptionCertificate` and `CreateSigningCertificate` in the existing static class `CertGen.cs`.
2. Add a new `string outFilePrefix = "server-"` parameter to both methods.
3. Change the generated file names to `{outFilePrefix}encryption.pfx` and `{outFilePrefix}signing.pfx` respectively, matching the requirement example.
4. Add input validation inside these methods to check if `notAfter <= notBefore`. If true, throw an `ArgumentException` stating that `not-after` must be greater than `not-before`.
