# GEMINI.md - AI Assistant Guidelines

> **[IMPORTANT] DOCUMENTATION STRUCTURE**
> This file contains **GLOBAL** agentic instructions and rules of engagement.
>
> - **Backend Technical Details:** See `src/ApogeeDev.IdentityProvider.Host/README.md`
> - **Frontend Technical Details:** See `src/idp-manager-app/README.md`
>
> **Do not add component-specific build commands or conventions here. Update the respective component READMEs instead.**

## 1. Global Rules & Safety

- **Secrets:** **NEVER** ask for or commit secret values like API keys or connection strings.
  - If a task requires a secret, ask the user to configure it using the .NET Secret Manager or `.env` files locally.
- **Context Awareness:** Before starting a task, always check the `README.md` in the specific subdirectory you are working in (`src/ApogeeDev.IdentityProvider.Host` or `src/idp-manager-app`) for the latest build commands, tech stack details, and coding conventions.

## 2. How to Prompt Me Effectively

To get the best results, please structure your requests clearly.

1.  **Be Specific:**

    > **Good:** "In `Login.cshtml`, the Google sign-in button is not working. I suspect the issue is in the `PostGoogleLoginHandler.cs`. Please investigate the handler and fix the logic."

2.  **Provide Context:** Include file paths, relevant code snippets, and any error messages.

3.  **Define Scope:** Clearly state the boundaries of the task.

    > **Good:** "Implement Microsoft login. This will involve:
    >
    > 1. Creating a `MicrosoftClaimsProcessor.cs`.
    > 2. Creating a `PostMicrosoftLoginHandler.cs`.
    > 3. Adding a new button to the `Login.cshtml` view."

4.  **Iterate:** For complex features, break the task down into smaller, sequential steps.

## 3. In Case of Errors

If I make a mistake, get stuck, or misunderstand a request:

- Tell me to **stop**.
- Use `git status` to see what files I've changed.
- Use `git restore <file_path>` to revert changes to a specific file if needed.
- Clarify the request with more context or a different approach.
