# Phase 1: Security CVE Remediation & Non-Breaking Minor Updates

> **Document ID:** `001_phase1_security_and_non_breaking_updates`  
> **Phase:** 1 of 3  
> **Status:** Ready for Execution  
> **Goal:** Eliminate all security vulnerabilities (CVEs) in both frontend and backend, fix pre-existing lint issues, and apply safe minor/patch servicing updates without introducing breaking API changes.

---

## 1. Scope & Impact Matrix

| Project / Component | Target Files | Key Changes |
| :--- | :--- | :--- |
| **Frontend Root** | [`pnpm-workspace.yaml`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/pnpm-workspace.yaml) | Update package overrides to patch critical `shell-quote` command injection (`CVE-2026-102422`), `brace-expansion` DoS, `browserslist`, `source-map-js`, and `postcss-selector-parser`. |
| **Frontend Codebase** | [`src/idp-manager-app/src/types/errors.js`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/idp-manager-app/src/types/errors.js)<br/>[`src/idp-manager-app/src/stores/clients.js`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/idp-manager-app/src/stores/clients.js) | Resolve pre-existing oxlint/eslint errors (empty file & unused declarations) so `pnpm lint` can pass cleanly. |
| **Frontend App** | [`src/idp-manager-app/package.json`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/idp-manager-app/package.json) | Upgrade `vue` (patches `@vue/server-renderer` XSS), `@vitejs/plugin-vue`, `vite-plugin-vue-devtools`, `bootstrap-icons`, `oxlint`, `eslint-plugin-oxlint`, `eslint-plugin-vue`, `prettier`, and `sass-embedded`. |
| **Backend Host** | [`src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj) | Upgrade `MongoDB.Driver` to 3.12.0 (patches `Snappier` & `SharpCompress`), `OpenTelemetry.AutoInstrumentation` to 1.17.0 (patches 4 transitive Otel CVEs), ASP.NET Core 10.0.13 servicing patches, Serilog 4.4.0, and Swashbuckle 10.3.0. |
| **Backend Tests** | [`src/ApogeeDev.IdentityProvider.Host.IntegrationTests/ApogeeDev.IdentityProvider.Host.IntegrationTests.csproj`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host.IntegrationTests/ApogeeDev.IdentityProvider.Host.IntegrationTests.csproj) | Upgrade `Testcontainers.MongoDb` to 4.16.0 (patches `SSH.NET` CVEs) and `Microsoft.AspNetCore.Mvc.Testing` to 10.0.13. |

---

## 2. Step-by-Step Implementation Instructions

### Step 1: Fix Pre-existing Frontend Lint Issues

`pnpm lint` currently fails due to 3 static analysis violations. Fix them before upgrading tooling:

1. **Delete or Export from Empty File:**
   - File: `src/idp-manager-app/src/types/errors.js`
   - Problem: Empty 0-byte file triggers `unicorn/no-empty-file`.
   - Action: Add a typedef export or remove the empty file if unused:
     ```javascript
     /**
      * @typedef {Object} ApiError
      * @property {string} [code]
      * @property {string} message
      */
     export default {}
     ```

2. **Clean Unused Declarations in Store:**
   - File: `src/idp-manager-app/src/stores/clients.js`
   - Problem: Line 82 declares unused `clientSecret` in destructuring; line 135 declares unused helper `applyFilters`.
   - Action:
     - On line 82: Change `const { clientSecret, ...c } = appClientData` to `const { clientSecret: _, ...c } = appClientData` (or omit destructuring if secret is omitted by property deletion).
     - On line 135: Export `applyFilters` or prefix with underscore `_applyFilters` if retained for future filtering.

3. **Verify Pre-upgrade Lint:**
   ```bash
   pnpm --filter idp-manager-app lint
   ```

---

### Step 2: Update `pnpm-workspace.yaml` Overrides

Edit [`pnpm-workspace.yaml`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/pnpm-workspace.yaml) to eliminate all transitive vulnerabilities flagged by `pnpm audit`:

```yaml
packages:
  - src/idp-manager-app

overrides:
  '@babel/core@<=7.29.0': '>=7.29.6'
  ajv@<6.14.0: '>=6.14.0'
  baseline-browser-mapping@<2.11.0: '>=2.11.0'
  brace-expansion: '>=5.0.12'
  browserslist: '>=4.28.7'
  esbuild@>=0.27.3 <0.28.1: '>=0.28.1'
  flatted@<3.4.0: '>=3.4.0'
  immutable@>=5.0.0 <5.1.5: '>=5.1.5'
  immutable@>=5.0.0-beta.1 <5.1.8: '>=5.1.8'
  js-yaml: '>=5.4.1'
  minimatch@<3.1.4: '>=3.1.4'
  nanoid@<3.3.18: '>=3.3.18'
  postcss@<=8.5.22: '>=8.5.23'
  postcss-selector-parser: '>=7.1.6'
  rollup@>=4.0.0 <4.59.0: '>=4.59.0'
  shell-quote: '>=1.11.0'
  source-map-js: '>=1.2.2'
```

---

### Step 3: Upgrade Safe Frontend Packages

Update [`src/idp-manager-app/package.json`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/idp-manager-app/package.json):

```json
  "dependencies": {
    "@popperjs/core": "^2.11.8",
    "bootstrap": "^5.3.8",
    "oidc-client-ts": "^3.5.0",
    "pinia": "^3.0.4",
    "vue": "^3.5.44",
    "vue-router": "^4.6.4"
  },
  "devDependencies": {
    "@eslint/js": "^9.39.4",
    "@prettier/plugin-oxc": "^0.2.3",
    "@vitejs/plugin-vue": "^6.0.10",
    "@vue/eslint-config-prettier": "^10.2.0",
    "bootstrap-icons": "^1.13.2",
    "eslint": "^9.39.4",
    "eslint-plugin-oxlint": "~1.88.0",
    "eslint-plugin-vue": "~10.11.1",
    "globals": "^16.5.0",
    "npm-run-all2": "^8.0.4",
    "oxlint": "~1.88.0",
    "prettier": "3.9.10",
    "sass-embedded": "^1.105.1",
    "vite": "^7.3.5",
    "vite-plugin-vue-devtools": "^8.2.1"
  }
```

**Execute Installation & Verification:**
```bash
pnpm install
pnpm audit
pnpm --filter idp-manager-app build
pnpm --filter idp-manager-app lint
```
*Expected Result:* `pnpm audit` reports **0 vulnerabilities**. Build and lint exit with code 0.

---

### Step 4: Upgrade Backend Security & Servicing Packages

#### A. Host Project Updates
Edit [`src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj):

```xml
  <ItemGroup>
    <PackageReference Include="MediatR" Version="12.5.0" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation" Version="10.0.13" Condition="'$(Configuration)' == 'Debug'" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="10.0.13" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.9">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="MongoDB.Driver" Version="3.12.0" />
    <PackageReference Include="MongoDB.Driver.Core.Extensions.DiagnosticSources" Version="2.1.0" />
    <PackageReference Include="MongoDB.EntityFrameworkCore" Version="9.0.3" />
    <PackageReference Include="OpenIddict.AspNetCore" Version="7.2.0" />
    <PackageReference Include="OpenIddict.MongoDb" Version="7.2.0" />
    <PackageReference Include="OpenIddict.Quartz" Version="7.2.0" />
    <PackageReference Include="OpenTelemetry.AutoInstrumentation" Version="1.17.0" />
    <PackageReference Include="OpenTelemetry.Exporter.Prometheus.AspNetCore" Version="1.19.1-beta.1" />
    <PackageReference Include="OpenTelemetry.Exporter.Console" Version="1.19.1" />
    <PackageReference Include="OpenTelemetry.Exporter.OpenTelemetryProtocol" Version="1.19.1" />
    <PackageReference Include="OpenTelemetry.Extensions.Hosting" Version="1.19.1" />
    <PackageReference Include="OpenTelemetry.Instrumentation.AspNetCore" Version="1.19.0" />
    <PackageReference Include="OpenTelemetry.Instrumentation.EntityFrameworkCore" Version="1.19.1-beta.1" />
    <PackageReference Include="OpenTelemetry.Instrumentation.Http" Version="1.19.0" />
    <PackageReference Include="Quartz.Extensions.Hosting" Version="3.15.0" />
    <PackageReference Include="Serilog" Version="4.4.0" />
    <PackageReference Include="Serilog.AspNetCore" Version="9.0.0" />
    <PackageReference Include="Serilog.Enrichers.Environment" Version="3.0.1" />
    <PackageReference Include="Serilog.Enrichers.Thread" Version="4.0.0" />
    <PackageReference Include="Serilog.Settings.Configuration" Version="9.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="10.3.0" />
    <PackageReference Include="System.Linq.Async" Version="6.0.3" />
  </ItemGroup>
```

#### B. Test Project Updates
Edit [`src/ApogeeDev.IdentityProvider.Host.IntegrationTests/ApogeeDev.IdentityProvider.Host.IntegrationTests.csproj`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host.IntegrationTests/ApogeeDev.IdentityProvider.Host.IntegrationTests.csproj):

```xml
  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.4" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.13" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
    <PackageReference Include="Testcontainers.MongoDb" Version="4.16.0" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
  </ItemGroup>
```

---

## 3. Verification Checklist & Success Criteria

Execute the following commands to confirm Phase 1 success:

- [ ] `dotnet restore`
- [ ] `dotnet build`
  - **Success criterion:** Build succeeded with **0 vulnerability warnings** (formerly 32 warnings).
- [ ] `pnpm audit`
  - **Success criterion:** `No vulnerabilities found`.
- [ ] `pnpm --filter idp-manager-app build`
  - **Success criterion:** Successful Vite build in `< 6s`.
- [ ] `pnpm --filter idp-manager-app lint`
  - **Success criterion:** Clean exit, zero errors.

---

## 4. Rollback Plan

If any unexpected regression occurs during Phase 1:
```bash
git checkout pnpm-lock.yaml pnpm-workspace.yaml
git checkout src/idp-manager-app/package.json
git checkout src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj
git checkout src/ApogeeDev.IdentityProvider.Host.IntegrationTests/ApogeeDev.IdentityProvider.Host.IntegrationTests.csproj
pnpm install
dotnet restore
```
