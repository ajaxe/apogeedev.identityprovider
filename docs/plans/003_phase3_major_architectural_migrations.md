# Phase 3: Major Architectural Migrations

> **Document ID:** `003_phase3_major_architectural_migrations`  
> **Phase:** 3 of 3  
> **Status:** Ready for Execution (after Phase 2)  
> **Goal:** Migrate libraries that contain breaking API changes, requiring source code refactoring across the CLI tool, MediatR pipeline, and frontend build/routing infrastructure.

---

## 1. Scope & Impact Matrix

| Project / Component | Target Files | Key Migrations |
| :--- | :--- | :--- |
| **CLI Tool** | [`src/SelfSignedCert/SelfSignedCert.csproj`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/SelfSignedCert/SelfSignedCert.csproj)<br/>[`src/SelfSignedCert/Program.cs`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/SelfSignedCert/Program.cs) | Migrate `System.CommandLine` from `2.0.0-beta4` to `2.0.13` (API rewritten from `SetHandler` to `SetAction` / `ParseResult`). |
| **Backend Core** | [`src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj)<br/>[`src/ApogeeDev.IdentityProvider.Host/Operations/RequestHandlers/LoggingBehavior.cs`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host/Operations/RequestHandlers/LoggingBehavior.cs) | Migrate `MediatR` from `12.5.0` to `14.2.0`; upgrade `System.Linq.Async` to `7.0.1`. |
| **Frontend Framework** | [`src/idp-manager-app/package.json`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/idp-manager-app/package.json)<br/>[`src/idp-manager-app/vite.config.js`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/idp-manager-app/vite.config.js) | Major upgrades: `vite 8`, `pinia 4`, `vue-router 5`, `npm-run-all2 9`, and `eslint 10`. |

---

## 2. Step-by-Step Implementation Instructions

### Step 1: Migrate `System.CommandLine` in `SelfSignedCert`

`System.CommandLine` graduated from beta to general release (`2.0.13`), breaking the old `SetHandler` convention.

1. **Update Project Reference:**
   In [`src/SelfSignedCert/SelfSignedCert.csproj`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/SelfSignedCert/SelfSignedCert.csproj):
   ```xml
   <ItemGroup>
     <PackageReference Include="System.CommandLine" Version="2.0.13" />
   </ItemGroup>
   ```

2. **Refactor [`src/SelfSignedCert/Program.cs`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/SelfSignedCert/Program.cs):**
   Replace the beta `SetHandler` implementation with the modern `SetAction` API:

   ```csharp
   using SelfSignedCert;
   using System.CommandLine;

   var rootCommand = new RootCommand("Self-signed certificate generator");

   var outFilePrefixOption = new Option<string>("--out-file-prefix")
   {
       Description = "Prefix for the output files"
   };
   outFilePrefixOption.Aliases.Add("-o");

   var notBeforeOption = new Option<DateTimeOffset?>("--not-before")
   {
       Description = "The not-before date"
   };
   notBeforeOption.Aliases.Add("-nb");

   var notAfterOption = new Option<DateTimeOffset?>("--not-after")
   {
       Description = "The not-after date"
   };
   notAfterOption.Aliases.Add("-na");

   rootCommand.AddOption(outFilePrefixOption);
   rootCommand.AddOption(notBeforeOption);
   rootCommand.AddOption(notAfterOption);

   rootCommand.SetAction(parseResult =>
   {
       string? outFilePrefix = parseResult.GetValue(outFilePrefixOption);
       DateTimeOffset? notBefore = parseResult.GetValue(notBeforeOption);
       DateTimeOffset? notAfter = parseResult.GetValue(notAfterOption);

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
   });

   return await rootCommand.Parse(args).InvokeAsync();
   ```

3. **Verify CLI Tool:**
   ```bash
   dotnet build src/SelfSignedCert/SelfSignedCert.csproj
   dotnet run --project src/SelfSignedCert/SelfSignedCert.csproj -- --help
   ```

---

### Step 2: Migrate `MediatR` 12 to 14

1. **Update Project Reference:**
   In [`src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj):
   ```xml
   <PackageReference Include="MediatR" Version="14.2.0" />
   <PackageReference Include="System.Linq.Async" Version="7.0.1" />
   ```

2. **Verify Pipeline Behavior:**
   Check [`src/ApogeeDev.IdentityProvider.Host/Operations/RequestHandlers/LoggingBehavior.cs`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host/Operations/RequestHandlers/LoggingBehavior.cs):
   ```csharp
   public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : notnull
   {
       // In MediatR 13+, RequestHandlerDelegate<TResponse>() is invoked as:
       // var response = await next();
       // or var response = await next(cancellationToken);
   }
   ```
   Ensure `where TRequest : notnull` conforms to MediatR 14 constraints and `services.AddMediatR(...)` in `Startup.cs` registers without compilation errors.

3. **Verify Backend Build:**
   ```bash
   dotnet build src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj
   ```

---

### Step 3: Frontend Major Infrastructure Upgrades

1. **Update `idp-manager-app` Dependencies:**
   In [`src/idp-manager-app/package.json`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/idp-manager-app/package.json):
   ```json
   "dependencies": {
     "@popperjs/core": "^2.11.8",
     "bootstrap": "^5.3.8",
     "oidc-client-ts": "^3.5.0",
     "pinia": "^4.0.3",
     "vue": "^3.5.44",
     "vue-router": "^5.4.0"
   },
   "devDependencies": {
     "@eslint/js": "^10.0.1",
     "@prettier/plugin-oxc": "^0.2.3",
     "@vitejs/plugin-vue": "^6.0.10",
     "@vue/eslint-config-prettier": "^10.2.0",
     "bootstrap-icons": "^1.13.2",
     "eslint": "^10.12.0",
     "eslint-plugin-oxlint": "~1.88.0",
     "eslint-plugin-vue": "~10.11.1",
     "globals": "^17.13.0",
     "npm-run-all2": "^9.0.3",
     "oxlint": "~1.88.0",
     "prettier": "3.9.10",
     "sass-embedded": "^1.105.1",
     "vite": "^8.3.4",
     "vite-plugin-vue-devtools": "^8.2.1"
   }
   ```

2. **Verify Pinia 4 and Vue Router 5 Compatibility:**
   - Review [`src/idp-manager-app/src/router/index.js`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/idp-manager-app/src/router/index.js) navigation guards and history modes.
   - Verify Pinia stores in [`src/idp-manager-app/src/stores`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/idp-manager-app/src/stores).
   - Test client creation and OAuth callback flows.

3. **Verify ESLint 10 Flat Config:**
   Review [`src/idp-manager-app/eslint.config.js`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/idp-manager-app/eslint.config.js) to ensure rules and plugin objects are compatible with ESLint 10.

4. **Execute Build & Lint:**
   ```bash
   pnpm install
   pnpm --filter idp-manager-app build
   pnpm --filter idp-manager-app lint
   ```

---

## 3. End-to-End Solution Verification

Once all three phases have been applied:

1. **Full .NET Solution Build:**
   ```bash
   dotnet restore
   dotnet build
   ```
   *Expected:* 0 Errors, 0 Warnings.

2. **Full Frontend Build & Lint:**
   ```bash
   pnpm --filter idp-manager-app build
   pnpm --filter idp-manager-app lint
   pnpm --filter idp-manager-app format
   ```
   *Expected:* Vite build bundles successfully, Oxlint/ESLint pass with 0 errors.

3. **Integration Test Suite:**
   ```bash
   # Ensure Docker desktop/daemon is running
   dotnet test
   ```
   *Expected:* All integration tests pass.

4. **Self-Signed Certificate Generator Sanity Check:**
   ```bash
   dotnet run --project src/SelfSignedCert/SelfSignedCert.csproj -- -o temp-test-cert-
   Remove-Item temp-test-cert-*
   ```

---

## 4. Rollback Plan

If regressions occur in Phase 3:
```bash
git checkout src/SelfSignedCert/
git checkout src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj
git checkout src/ApogeeDev.IdentityProvider.Host/Operations/RequestHandlers/LoggingBehavior.cs
git checkout src/idp-manager-app/package.json
git checkout pnpm-lock.yaml
pnpm install
dotnet restore
```
