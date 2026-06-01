# Oauth Client Credential Flow

## Goal
For existing OIDC custom IdP, add support for client credential flow using OpenIdDict framework that is secure anf follows best practices.

## Scope

* Add secure implementation of **client credentials** OAuth flow using OpenIdDict framework.
* Update **IdP Manager App** to allow registration of clients using _client credential flow_, default selected option is _Authorization Code with PKCE_.
* Plan testing strategy for client credential flow. Prefer automated testng plan over manual. Manual testing is last option.
* Update _AGENTS.md_ to reflect _client credentials flow_ implementation.
* Update _README.md_ to reflect _client credentials flow_ implementation.
