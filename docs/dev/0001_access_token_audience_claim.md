# 0001: Setting Access Token Audience (`aud`) Claim to Requesting `client_id`

- **Status:** Accepted
- **Date:** 2026-11-14
- **Affected Components:**
  - `src/ApogeeDev.IdentityProvider.Host/Helpers/Authentication/AttachClientIdHandler.cs` (Identity Provider Sign-In Handler)
  - `src/ApogeeDev.IdentityProvider.Host/Helpers/Authentication/AuthServerExtension.cs` (OpenIddict Server Registration)
  - Upstream Resource Servers (e.g., `ApogeeDev.Sample.Backend.Host`)

---

## 1. Issue Description

In **OpenIddict**, the `aud` (Audience) claim in a generated `access_token` is only populated when `SetResources(...)` is explicitly called on the `ClaimsPrincipal` (or `ClaimsIdentity`) during the sign-in flow (`/connect/authorize` or `/connect/token`).

Currently, `ApogeeDev.IdentityProvider.Host`:
1. Only registers standard OIDC identity scopes (`address`, `profile`, `email`, `phone`, `roles`, `offline_access`) in `AuthServerExtension.ConfigureOpenIdDictServer`.
2. Does not allow defining custom scopes or resource indicators mapped to specific upstream resource servers.
3. Attaches the requesting client's identifier as a `client_id` claim via `AttachClientIdHandler` (`context.AccessTokenPrincipal.SetClaim(Claims.ClientId, clientId)`), but never calls `SetResources(...)`.

As a result, issued `access_token` JWTs omit the `aud` claim entirely. Upstream resource servers using `Microsoft.AspNetCore.Authentication.JwtBearer` cannot enforce standard JWT audience validation (`ValidateAudience = true` / `options.Audience`) unless an `aud` claim is present in the token.

---

## 2. Evaluated Options

| Option | Mechanism | Pros | Cons |
| :--- | :--- | :--- | :--- |
| **1. Configure `Audiences` / `Resources` per Client Registration** | Add a `string[] Audiences` property to `AppClientData`, store it in `OpenIddictApplicationDescriptor.Properties`, and call `context.AccessTokenPrincipal.SetResources(audiences)` in a `ProcessSignInContext` handler. | Supports decoupling client IDs from API resource identifiers and allows one client to target multiple distinct APIs. | Requires schema, validation, API, and UI (`idp-manager-app`) updates for client management. |
| **2. Default `aud` to the Requesting `client_id` (Selected)** | In `AttachClientIdHandler` (`IOpenIddictServerHandler<ProcessSignInContext>`), set the requesting `client_id` as the default audience on `context.AccessTokenPrincipal` (`SetAudiences(clientId)`) when no explicit audiences are assigned. | Zero database schema, API, or UI changes required; every issued `access_token` automatically includes `"aud": "<client_id>"`; enables standard `ValidateAudience = true` in upstream APIs. | Couples the expected `aud` claim in the upstream API to the client's `client_id` (requires `ValidAudiences` if multiple clients share one backend API). |
| **3. RFC 8707 `resource` Indicator Parameter** | Require OIDC clients to pass an explicit `resource=<api-uri>` query/form parameter on `/connect/authorize` and `/connect/token`, and call `principal.SetResources(request.GetResources())` in `OAuthController`. | Standards-based per-request audience selection without IdP UI changes. | Requires every client to configure and send custom `resource` parameters on both authorization and token requests. |

---

## 3. Reason for the Choice

We selected **Option 2 (Defaulting `aud` to the Requesting `client_id`)** for the following architectural reasons:

1. **1:1 Client-to-Backend API Topology:**
   In our current architecture, each registered OIDC client (such as a Backend-for-Frontend proxy or service client) accesses a single corresponding backend API within the same logical application boundary. Using `client_id` as the audience identifier directly aligns with this 1:1 boundary (similar to how Google OIDC and ZITADEL project/client audiences operate).
2. **No Scope or Client Management Overhead:**
   `ApogeeDev.IdentityProvider.Host` intentionally keeps client registration simple without complex API Resource / Scope matrices. Defaulting `aud` to `client_id` inside `AttachClientIdHandler` requires no changes to `AppClientOptions`, `AppClientData`, MongoDB documents, or the `idp-manager-app` UI.
3. **Seamless Upstream `JwtBearer` Validation:**
   Upstream resource servers can enable `ValidateAudience = true` and set `Authentication:Audience` to the registered `ClientId`, achieving full cryptographic and audience binding with standard `JwtBearer` middleware.
4. **Forward-Compatible with Explicit Resources:**
   By guarding the assignment with `if (!context.AccessTokenPrincipal.GetAudiences().Any())`, the handler sets `client_id` as a fallback default. If future requirements introduce explicit resource scopes or per-client audience lists (which OpenIddict's `PrepareAccessTokenPrincipal` translates from `Resources` into `Audiences`), those explicit audiences will naturally take precedence without breaking existing clients.

---

## 4. Resolution Steps

### Step 1: Update `AttachClientIdHandler` in `ApogeeDev.IdentityProvider.Host`
In `src/ApogeeDev.IdentityProvider.Host/Helpers/Authentication/AttachClientIdHandler.cs`, call `SetAudiences(clientId)` on `context.AccessTokenPrincipal` when no audiences have been explicitly set:

```csharp
using OpenIddict.Abstractions;
using OpenIddict.Server;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Server.OpenIddictServerEvents;

namespace ApogeeDev.IdentityProvider.Host.Helpers.Authentication;

public class AttachClientIdHandler(ILogger<AttachClientIdHandler> logger)
    : IOpenIddictServerHandler<ProcessSignInContext>
{
    public ValueTask HandleAsync(ProcessSignInContext context)
    {
        string? clientId = context.Request?.ClientId;

        if (string.IsNullOrWhiteSpace(clientId))
        {
            logger.LogInformation("Client ID is missing in token request. Skipping client_id claim attachment.");
            return ValueTask.CompletedTask;
        }

        if (context.AccessTokenPrincipal is not null)
        {
            if (!context.AccessTokenPrincipal.HasClaim(Claims.ClientId, clientId))
            {
                logger.LogInformation("Attaching Client ID {ClientId} as '{ClaimType}' claim to AccessTokenPrincipal.",
                    clientId, Claims.ClientId);
                context.AccessTokenPrincipal.SetClaim(Claims.ClientId, clientId);
            }

            if (!context.AccessTokenPrincipal.GetAudiences().Any())
            {
                logger.LogInformation("Setting default audience (aud) to Client ID {ClientId} on AccessTokenPrincipal.",
                    clientId);
                context.AccessTokenPrincipal.SetAudiences(clientId);
            }
        }

        return ValueTask.CompletedTask;
    }
}
```

### Step 2: Enable Audience Validation in Upstream Resource Servers
In upstream APIs (such as `ApogeeDev.Sample.Backend.Host/Program.cs`), enable `ValidateAudience = true` and configure `Authentication:Audience` to match the BFF/client's `ClientId`:

```csharp
options.Audience = builder.Configuration["Authentication:Audience"]; // Set to the BFF's ClientId
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    NameClaimType = "name",
    RoleClaimType = "roles",
    ClockSkew = TimeSpan.FromMinutes(1)
};
```
