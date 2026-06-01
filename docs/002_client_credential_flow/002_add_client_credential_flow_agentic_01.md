# Phase 1: Backend Implementation for Client Credentials Flow

## Goal
Implement the core OpenIddict configuration and token endpoint logic in the `ApogeeDev.IdentityProvider.Host` project to support the Client Credentials flow.

## Scope
1. **OpenIddict Configuration**
   - File: `src/ApogeeDev.IdentityProvider.Host/Helpers/Authentication/AuthServerExtension.cs`
   - Action: Update `ConfigureOpenIdDictServer` to call `.AllowClientCredentialsFlow()` to enable the grant type at the server level.

2. **Token Endpoint Processing**
   - File: `src/ApogeeDev.IdentityProvider.Host/Controllers/OAuthController.cs`
   - Action: Update the `Exchange` method (which handles `/connect/token`) to process requests where `request.IsClientCredentialsGrantType()` is true.
   - Action: Create a generic `ClaimsPrincipal` representing the client application. This involves using the `request.ClientId` and assigning appropriate OpenIddict scopes/destinations for an application (non-user) token.
   
   **Code Snippet:**
   ```csharp
   if (request.IsClientCredentialsGrantType())
   {
       // Create a new ClaimsIdentity for the client
       var identity = new ClaimsIdentity(
           authenticationType: TokenValidationParameters.DefaultAuthenticationType,
           nameType: Claims.Name,
           roleType: Claims.Role);

       // Add the client identifier as the subject claim
       identity.AddClaim(Claims.Subject, request.ClientId ?? throw new InvalidOperationException());
       identity.AddClaim(Claims.Name, request.ClientId);

       var principal = new ClaimsPrincipal(identity);
       principal.SetScopes(request.GetScopes());

       return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
   }
   ```

## Shared Context: Testing Setup
*Note: Pre-requisite Phase 0 has established the `ApogeeDev.IdentityProvider.Host.IntegrationTests` project using `WebApplicationFactory` and `Testcontainers.MongoDb`.*

3. **Backend Testing**
   - **Test Case Implementation:**
     - **Arrange:** Use the OpenIddict application manager in the established test project to seed a test client configured with `Permissions.GrantTypes.ClientCredentials` and `Permissions.Endpoints.Token`.
     - **Act:** Use the test `HttpClient` to send a `POST` request to `/connect/token` with `grant_type=client_credentials`, `client_id`, and `client_secret`.
     - **Assert:** Verify the response is `200 OK` and contains a valid JWT access token.
