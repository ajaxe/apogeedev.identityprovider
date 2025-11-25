# IDP Manager App (Frontend)

Single Page Application (SPA) for managing the Identity Provider.

## Tech Stack

- **Framework:** Vue 3 (Composition API with `<script setup>`)
- **Build Tool:** Vite
- **State Management:** Pinia
- **Styling:** Bootstrap 5 (via `bootstrap` and `bootstrap-icons`)
- **Auth:** oidc-client-ts
- **Routing:** vue-router

## Setup & Run

```bash
# Install
pnpm install

# Dev Server
pnpm dev

# Build
pnpm build
```

## Development Workflow

- **Linting:** `pnpm lint`
- **Formatting:** `pnpm format`
  > **Note:** Always run lint and format before committing.

## Coding Conventions

- Use `<script setup>` syntax.
- Ensure all new components are responsive.
- Configuration in `.env.development` and `.env.production`.
