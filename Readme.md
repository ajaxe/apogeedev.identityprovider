# Apogeedev.IdentityProvider

OpenIddict based Identity Provider

## Docker build

Backend docker build command:

```bash
docker build . -f build/Dockerfile --network=host --tag apogee-dev/identity-provider:local
```

SPA docker build command

```bash
docker build . -f build/Dockerfile.spa --network=host --tag apogee-dev/identity-provider-spa:local
```
