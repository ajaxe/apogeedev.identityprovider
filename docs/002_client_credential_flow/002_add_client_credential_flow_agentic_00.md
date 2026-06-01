# Phase 0: Pre-requisite - Integration Testing Infrastructure Setup

## Goal
Establish the backend integration testing infrastructure necessary to validate OpenIddict endpoints such as `/connect/token`.

## Scope
1. **Create Test Project**
   - Action: Scaffold a new xUnit project named `ApogeeDev.IdentityProvider.Host.IntegrationTests`.
   - Action: Add the project to the solution file (`apogeedev.identityprovider.sln`).
   - Action: Add a project reference to `ApogeeDev.IdentityProvider.Host`.

2. **Setup Dependencies**
   - Action: Install `Microsoft.AspNetCore.Mvc.Testing` to utilize `WebApplicationFactory` for bootstrapping the API in memory.
   - Action: Install `Testcontainers.MongoDb` for database testing.

3. **Database Configuration for Tests**
   - Action: Implement a custom `WebApplicationFactory<Startup>`.
   - Action: Override the DI container to use an ephemeral MongoDB instance spun up by `Testcontainers` to isolate integration tests from the main database.
