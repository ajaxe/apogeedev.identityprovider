# GEMINI.md - AI Assistant Guidelines

This document provides guidelines for the Gemini AI assistant to ensure it can contribute to this project effectively, safely, and in accordance with our standards.

## 1. Project Setup & Configuration

Before performing any task, ensure the project is correctly set up.

**Initial Setup:**
To restore dependencies and build the project, run:

```bash
dotnet restore
dotnet build
```

**Configuration & Secrets:**
The main configuration is in `src/ApogeeDev.IdentityProvider.Host/appsettings.json`. For development, overrides are in `appsettings.Development.json`.

> **NEVER** ask for secret values like API keys or connection strings. If a task requires a secret, please ask me to configure it using the .NET Secret Manager tool:

> ```bash
> dotnet user-secrets set "KeyName" "Value" --project "src/ApogeeDev.IdentityProvider.Host"
> ```

## 2. Development Workflow

Follow these standard procedures for building, testing, and running the application.

**Building the Code:**
Always ensure the code builds successfully after any change.

```bash
dotnet build
```

**Running the Application:**
To run the main host project for testing or debugging:

```bash
dotnet run --project src/ApogeeDev.IdentityProvider.Host/ApogeeDev.IdentityProvider.Host.csproj
```

**Testing:**
This project does not currently have a dedicated test project. When adding new features that can be unit tested, please create a corresponding test project and add tests. When tests are available, run them with:

```bash
dotnet test
```

**Code Formatting:**
Before finalizing changes, apply standard .NET code formatting.

```bash
dotnet format
```

## 3. Coding Conventions & Style Guide

Adherence to the existing code style is critical.

- **Analyze First:** Before writing any code, read the surrounding files to understand the established patterns, naming conventions, and style.
- **Naming:** Follow existing C# conventions (e.g., `PascalCase` for classes and methods, `camelCase` for local variables).
- **Style:** Mimic the brace style, use of `var`, and `using` statement organization found in existing files.
- **File Structure:** Place new files in the appropriate directories based on their function (e.g., new controllers in `Controllers`, new services in a corresponding `Services` or `Operations` folder).

## 4. Architectural Overview

This is an ASP.NET Core Identity Provider application.

- **Host Project:** `ApogeeDev.IdentityProvider.Host` is the main executable web application.
- **Controllers:** Handle incoming HTTP requests (e.g., `OAuthController.cs`).
- **Operations (CQRS Pattern):** The `Operations` directory suggests a CQRS (Command Query Responsibility Segregation) pattern, likely using a library like MediatR.
  - **Request Handlers:** Contain the core business logic for specific actions (e.g., `PostGoogleLoginHandler.cs`). New business logic should be added here.
  - **Processors:** Contain logic for processing claims from external providers (e.g., `GoogleClaimsProcessor.cs`).
- **Models:**
  - `ViewModels`: Data transfer objects (DTOs) for views.
  - `DatabaseModels`: Entities for the database.
- **Data:** The `ApplicationDbContext.cs` file indicates usage of Entity Framework Core for database interaction.

## 5. How to Prompt Me Effectively

To get the best results, please structure your requests clearly.

1. **Be Specific:**

   > **Bad:** "Fix the login page."
   >
   > **Good:** "In `Login.cshtml`, the Google sign-in button is not working. I suspect the issue is in the `PostGoogleLoginHandler.cs`. Please investigate the handler and fix the logic."

2. **Provide Context:** Include file paths, relevant code snippets, and any error messages.

3. **Define Scope:** Clearly state the boundaries of the task.

   > **Bad:** "Add Microsoft login."
   >
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
