# Identity Provider Host (Backend)

ASP.NET Core Identity Provider using OpenIddict and MongoDB.

## Tech Stack

- **Framework:** ASP.NET Core
- **Database:** MongoDB (via `MongoDB.Driver` & `MongoDB.EntityFrameworkCore`)
- **Architecture:** CQRS with MediatR
  - **Operations:** Contains `RequestHandlers` (business logic) and `Processors`.
- **Observability:** OpenTelemetry (Prometheus, OTLP)

## Setup & Run

```bash
# Restore & Build
dotnet restore
dotnet build

# Run
dotnet run --project ApogeeDev.IdentityProvider.Host.csproj
```

## Development Workflow

- **Formatting:** `dotnet format`
- **Testing:** `dotnet test` (when tests are added)
- **Secrets:** Use `dotnet user-secrets` for sensitive keys.
  ```bash
  dotnet user-secrets set "KeyName" "Value" --project "ApogeeDev.IdentityProvider.Host.csproj"
  ```

## Coding Conventions

- **Naming:** PascalCase for classes/methods, camelCase for locals.
- **Style:** Mimic the brace style, use of `var`, and `using` statement organization found in existing files.
- **File Structure:** Place new files in the appropriate directories based on their function (e.g., new controllers in `Controllers`, new services in a corresponding `Services` or `Operations` folder).

## Notes

Starting from v2.28.0 of MongoDB.Driver the dlls are strong named, if latest _MongoDB.EntityFrameworkCore_ is used it breaks compatibility with reference _for \_OpenIddict.MongoDb_ library. To allow the project to build correctly we
will fix EFCore version to _8.0.3_.

### Secrets.json config sample

Following is the sample of _secrets.json_ file:

```json
{
  "AppOptions": {
    "Username": "",
    "Password": "",
    "MongoDbConnection": "",
    "DatabaseName": "",
    "EncryptionKeyPassword": "",
    "AppManagerEmails": []
  },
  "AppClientOptions": {
    "Clients": [
      {
        "ApplicationType": "web",
        "DisplayName": "Local App Client Manager",
        "ClientId": "app_client_manager_local",
        "ClientSecret": "",
        "ClientType": "public",
        "RedirectUris": ["https://localhost:5173/auth-callback"],
        "PostLogoutRedirectUris": ["https://localhost:5173/clients"]
      }
    ]
  },
  "OAuthWebProviderOptions": {
    "Providers": [
      {
        "Name": "github",
        "RedirectUri": "callback/login/github",
        "ClientId": "",
        "ClientSecret": "",
        "Scopes": "read:user user:email"
      },
      {
        "Name": "google",
        "RedirectUri": "callback/login/google",
        "ClientId": "",
        "ClientSecret": "",
        "Scopes": "profile email"
      }
    ]
  }
}
```
