# Phase 3: Validation and Verification

## Objective
Build, run, and verify the CLI behaves correctly under various scenarios.

## Steps
1. Run the app with no arguments: `dotnet run --project src/SelfSignedCert`. Verify default filenames (`server-encryption.pfx`, `server-signing.pfx`) are created.
2. Run with custom prefix: `dotnet run --project src/SelfSignedCert -- -o myapp-`. Verify `myapp-encryption.pfx` and `myapp-signing.pfx` are created.
3. Run with date parameters to ensure successful parsing: `dotnet run --project src/SelfSignedCert -- -nb "2025-01-01" -na "2028-01-01"`.
4. Test date validation failure: `dotnet run --project src/SelfSignedCert -- -nb "2028-01-01" -na "2025-01-01"`. Verify the app prints the validation error gracefully.
