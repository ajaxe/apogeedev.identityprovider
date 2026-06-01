# Phase 4: Documentation Updates

## Goal
Update project documentation to reflect the new capabilities regarding the Client Credentials flow.

## Scope
1. **Update `AGENTS.md`**
   - File: `AGENTS.md`
   - Action: Document the newly added support for the Client Credentials flow. Provide context on how this flow operates within the OpenIddict implementation for AI assistants to reference in the future.

2. **Update `README.md`**
   - File: `Readme.md` (or relevant `README.md` in subdirectories depending on exact location).
   - Action: Include documentation for developers explaining that the IdP now supports Machine-to-Machine (M2M) authentication via the Client Credentials flow.
   - Action: Provide a basic example/curl command showing how to request a token using `client_id` and `client_secret` from the `/connect/token` endpoint.
   - Action: Update the existing mermaid.js diagram to include the new Client Credentials flow.
