# Phase 2: .NET 10 Ecosystem Alignment & OpenIddict / Quartz Migration

> **Document ID:** `002_phase2_dotnet10_ecosystem_and_openiddict_quartz`  
> **Phase:** 2 of 3  
> **Status:** Ready for Execution (after Phase 1)  
> **Goal:** Coordinate the upgrade of the data layer to Entity Framework Core 10, align Serilog on ASP.NET Core 10, update OpenIddict to 7.7.2, and migrate Quartz from 3.x to 4.x.

---

## 1. Context & Architectural Coupling

Phase 2 handles a tightly-coupled set of packages that must be upgraded simultaneously:

1. **The MongoDB + EF Core Interlock:**
   - The repository targets `net10.0`, but currently references EF Core 9 (`MongoDB.EntityFrameworkCore 9.0.3` and `Microsoft.EntityFrameworkCore.Design 9.0.9`).
   - `MongoDB.EntityFrameworkCore 10.0.4` targets `net10.0` and requires `Microsoft.EntityFrameworkCore >= 10.0.11` and `MongoDB.Driver >= 3.11.2`.
   - `OpenIddict.MongoDb 7.7.2` on `net10.0` requires `MongoDB.Driver >= 3.10.0`.
   - With `MongoDB.Driver` at `3.12.0` (from Phase 1), both EF Core 10 and OpenIddict 7.7.2 can now coexist harmoniously without strong-naming assembly mismatches.

2. **The OpenIddict + Quartz 4 Dependency:**
   - When targeting `net10.0`, NuGet package analysis reveals that `OpenIddict.Quartz 7.7.2` declares an explicit dependency on `Quartz [4.0.0, )` (unlike on .NET 8/9 where it accepted Quartz 3.x).
   - Therefore, bumping OpenIddict to 7.7.2 **mandates** upgrading `Quartz.Extensions.Hosting` from `3.15.0` to `4.5.0`.

---

## 2. Step-by-Step Implementation Instructions

### Step 1: Update Host Project File

Edit [`src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj):

```xml
  <ItemGroup>
    <!-- EF Core 10 Alignment -->
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.13">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="MongoDB.EntityFrameworkCore" Version="10.0.4" />
    <PackageReference Include="MongoDB.Driver.Core.Extensions.DiagnosticSources" Version="3.0.0" />

    <!-- OpenIddict 7.7.2 Suite -->
    <PackageReference Include="OpenIddict.AspNetCore" Version="7.7.2" />
    <PackageReference Include="OpenIddict.MongoDb" Version="7.7.2" />
    <PackageReference Include="OpenIddict.Quartz" Version="7.7.2" />

    <!-- Quartz 4.x Migration -->
    <PackageReference Include="Quartz.Extensions.Hosting" Version="4.5.0" />

    <!-- Serilog .NET 10 Alignment -->
    <PackageReference Include="Serilog.AspNetCore" Version="10.0.0" />
    <PackageReference Include="Serilog.Settings.Configuration" Version="10.0.1" />
  </ItemGroup>
```

---

### Step 2: Audit and Adapt `Startup.cs` for Quartz 4

Inspect [`src/ApogeeDev.IdentityProvider.Host/Startup.cs`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host/Startup.cs#L120-L126):

```csharp
// Current code (lines 120-125):
services.AddQuartz(options =>
{
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});
services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
```

#### Quartz 4 Compatibility Notes:
- In Quartz 4, `options.UseSimpleTypeLoader()` and `options.UseInMemoryStore()` remain valid standard configuration methods.
- Verify `services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true)` compiles without obsolescence warnings.
- OpenIddict 7.7.2's `services.AddOpenIddict().AddCore(options => options.UseQuartz())` automatically registers Quartz job details and pruning triggers for authorization and token cleanup.

---

### Step 3: Update Test Project Runners

Edit [`src/ApogeeDev.IdentityProvider.Host.IntegrationTests/ApogeeDev.IdentityProvider.Host.IntegrationTests.csproj`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host.IntegrationTests/ApogeeDev.IdentityProvider.Host.IntegrationTests.csproj):

```xml
  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="10.1.0" />
    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.13" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="18.11.0" />
    <PackageReference Include="Testcontainers.MongoDb" Version="4.16.0" />
    <PackageReference Include="xunit" Version="2.9.3" />
    <PackageReference Include="xunit.runner.visualstudio" Version="4.0.1" />
  </ItemGroup>
```

---

### Step 4: Update Documentation Reference

In [`src/ApogeeDev.IdentityProvider.Host/README.md`](file:///C:/CodeWorkspace/projects/apogeedev.identityprovider/src/ApogeeDev.IdentityProvider.Host/README.md#L41-L43), an outdated note states:
> *"Starting from v2.28.0 of MongoDB.Driver the dlls are strong named, if latest MongoDB.EntityFrameworkCore is used it breaks compatibility with reference for OpenIddict.MongoDb library. To allow the project to build correctly we will fix EFCore version to 8.0.3."*

Update this section to document that the solution now runs `net10.0` with `MongoDB.EntityFrameworkCore 10.0.4`, `MongoDB.Driver 3.12.0`, and `OpenIddict 7.7.2`.

---

## 3. Verification Checklist & Success Criteria

Execute the following verification steps:

- [ ] `dotnet restore`
  - **Success criterion:** Clean restore across all projects without package downgrade warnings.
- [ ] `dotnet build`
  - **Success criterion:** Build succeeded with `0 Error(s)` and `0 Warning(s)`.
- [ ] `dotnet test`
  - **Prerequisite:** Docker daemon must be running (required for `Testcontainers.MongoDb`).
  - **Command:** `dotnet test --no-build`
  - **Success criterion:** All integration tests in `ApogeeDev.IdentityProvider.Host.IntegrationTests` pass.

---

## 4. Rollback Plan

If Quartz 4 or EF Core 10 causes runtime or DI initialization issues:
```bash
git checkout src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj
git checkout src/ApogeeDev.IdentityProvider.Host.IntegrationTests/ApogeeDev.IdentityProvider.Host.IntegrationTests.csproj
git checkout src/ApogeeDev.IdentityProvider.Host/Startup.cs
dotnet restore
dotnet build
```
