# GEMINI.md - AI Assistant Guidelines

This document provides guidelines for the Gemini AI assistant to ensure it can contribute to this project effectively, safely, and in accordance with our standards.

## 1. Project Setup & Configuration

Before performing any task, ensure the project is correctly set up.

**Initial Setup:**
To restore dependencies and build the project, run:

```bash
# Backend
dotnet restore
dotnet build

# Frontend
cd src/idp-manager-app
pnpm install
pnpm build
```

**Configuration & Secrets:**

- **Backend:** `src/ApogeeDev.IdentityProvider.Host/appsettings.json`. Overrides in `appsettings.Development.json`.
- **Frontend:** `src/idp-manager-app/.env.development` and `.env.production`.

> **NEVER** ask for secret values like API keys or connection strings. If a task requires a secret, please ask me to configure it using the .NET Secret Manager tool:

> ```bash
> dotnet user-secrets set "KeyName" "Value" --project "src/ApogeeDev.IdentityProvider.Host"
> ```

## 2. Development Workflow

Follow these standard procedures for building, testing, and running the application.

**Running the Application:**

- **Backend (ASP.NET Core):**

  ```bash
  dotnet run --project src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj
  ```

- **Frontend (Vue 3):**
  ```bash
  cd src/idp-manager-app
  pnpm dev
  ```

**Testing:**
This project does not currently have a dedicated test project. When adding new features that can be unit tested, please create a corresponding test project and add tests. When tests are available, run them with:

```bash
dotnet test
```

**Code Formatting:**

- **Backend:**
  ```bash
  dotnet format
  ```
- **Frontend:**
  ```bash
  cd src/idp-manager-app
  pnpm format
  pnpm lint
  ```

## 3. Coding Conventions & Style Guide

Adherence to the existing code style is critical.

### General

- **Analyze First:** Before writing any code, read the surrounding files to understand the established patterns, naming conventions, and style.

### Backend (C# / .NET)

- **Naming:** Follow existing C# conventions (e.g., `PascalCase` for classes and methods, `camelCase` for local variables).
- **Style:** Mimic the brace style, use of `var`, and `using` statement organization found in existing files.
- **File Structure:** Place new files in the appropriate directories based on their function (e.g., new controllers in `Controllers`, new services in a corresponding `Services` or `Operations` folder).

### Frontend (Vue 3 / JS)

- **Framework:** Vue 3 with Composition API (`<script setup>`).
- **Build Tool:** Vite.
- **State Management:** Pinia.
- **Styling:** Bootstrap 5 (via `bootstrap` and `bootstrap-icons`).
- **Linting:** ESLint + Prettier + Oxlint. Always run `pnpm lint` and `pnpm format` before committing.

## 4. Architectural Overview

This is a hybrid application with an ASP.NET Core backend and a Vue 3 frontend.

### Backend: `ApogeeDev.IdentityProvider.Host`

- **Type:** ASP.NET Core Identity Provider (OpenIddict).
- **Database:** MongoDB (via `MongoDB.Driver` and `MongoDB.EntityFrameworkCore`).
- **Architecture:** CQRS (Command Query Responsibility Segregation) using MediatR.
  - **Operations:** Contains `RequestHandlers` (business logic) and `Processors`.
- **Observability:** OpenTelemetry (Prometheus, OTLP).

### Frontend: `idp-manager-app`

- **Type:** Single Page Application (SPA).
- **Location:** `src/idp-manager-app`.
- **Key Libraries:** `oidc-client-ts` for authentication, `vue-router` for routing.

## 5. How to Prompt Me Effectively

To get the best results, please structure your requests clearly.

1. **Be Specific:**

   > **Good:** "In `Login.cshtml`, the Google sign-in button is not working. I suspect the issue is in the `PostGoogleLoginHandler.cs`. Please investigate the handler and fix the logic."

2. **Provide Context:** Include file paths, relevant code snippets, and any error messages.

3. **Define Scope:** Clearly state the boundaries of the task.

   > **Good:** "Implement Microsoft login. This will involve:
   >
   > 1. Creating a `MicrosoftClaimsProcessor.cs`.
   > 2. Creating a `PostMicrosoftLoginHandler.cs`.
   > 3. Adding a new button to the `Login.cshtml` view."

4. **Iterate:** For complex features, break the task down into smaller, sequential steps.

## 6. In Case of Errors

If I make a mistake, get stuck, or misunderstand a request:

- Tell me to **stop**.
- Use `git status` to see what files I've changed.
- Use `git restore <file_path>` to revert changes to a specific file if needed.
- Clarify the request with more context or a different approach.
