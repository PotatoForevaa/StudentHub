# Copilot Instructions for StudentHub Frontend

Purpose: Give an AI coding agent immediate, actionable context for contributing to the StudentHub React + Vite frontend so suggestions and code edits match the project's structure and conventions.

- **Project type & entry points:** This is a Vite + React + TypeScript frontend. Top-level app components live in `src/app/` (notably `App.tsx`, `Header.tsx`, `Main.tsx`). Use these files to understand routing and layout.

- **Feature organization:** Features are grouped under `src/features/` by domain:
  - `auth/` — authentication pages, form components, and hooks (`useAuthForm.ts`).
  - `profile/` — profile page(s) in `pages/profile.tsx`.
  - `projects/` — project pages and components.

- **Shared primitives & contexts:** Look to `src/shared/` and `src/context/` for reusable patterns:
  - `src/shared/components/PrivateRoute.tsx` — route protection; consult `src/context/AuthContext.tsx` to implement auth-aware behaviour.
  - `src/context/ProjectContext.tsx` — global project state management.

- **API layer:** Backend calls are centralized under `src/services/api/`:
  - `base.ts` configures the HTTP client (token handling, base URL). Prefer adding endpoints to `authService.ts` and `projectService.ts` rather than calling `fetch` directly.
  - Types shared by services live in `api.types.ts`.

- **Types placement:** Domain types live in `src/types/` (e.g., `User.tsx`, `Project.tsx`). Use these types for component props, service responses and context values.

- **Conventions & patterns to follow:**
  - Components are `.tsx`. Keep presentation in `components/` and page-level routing in `pages/`.
  - Hooks live under `features/*/hooks/`. Add new feature-specific hooks there.
  - Use context providers (e.g., `AuthContext`, `ProjectContext`) for cross-cutting state rather than prop-drilling.

- **Where to change authentication flow:**
  - Inspect `src/context/AuthContext.tsx` for login/logout/token lifecycle; update `src/services/api/base.ts` if token header injection needs changes.
  - Login/registration UI is under `src/features/auth/components/` and pages `src/features/auth/pages/` — follow the existing `AuthForm.tsx` and `useAuthForm.ts` patterns when adding fields.

- **Routing & private pages:** `PrivateRoute.tsx` is the canonical guard. Ensure new pages requiring auth are wrapped by it (or use the context to redirect in `useEffect`).

- **Build & dev commands (common Vite defaults):** If you need to run or test locally, use these (PowerShell):
  ```powershell
  npm install
  npm run dev
  npm run build
  npm run preview
  ```
  If package scripts differ, prefer the `package.json` scripts.

- **Code style & linting:** Project has `eslint.config.js` and TypeScript config files. Follow existing lint rules. Run `npm run lint` if present.

- **Adding API endpoints:** Prefer extending `src/services/api/*Service.ts` and exporting typed functions. Example pattern: add `export const fetchProjects = async (): Promise<Project[]> => { return api.get('/projects') }` using the configured client in `base.ts`.

- **Tests & missing areas:** No test files visible under `src/` — when suggested, propose adding unit tests for hooks and context providers (`react-testing-library`) and integration tests for service clients (mocking HTTP layer).

- **Files to inspect first when debugging a feature:**
  - UI issues: `src/app/*`, `src/features/*/components/*` and `src/shared/components/*`.
  - Auth and permissions: `src/context/AuthContext.tsx`, `src/shared/components/PrivateRoute.tsx`, `src/services/api/base.ts`.
  - API errors/flows: `src/services/api/*.ts` and `src/context/ProjectContext.tsx`.

- **Examples to follow when contributing:**
  - Add feature-level hooks like `src/features/auth/hooks/useAuthForm.ts` when encapsulating form state.
  - Keep API call functions in `authService.ts` / `projectService.ts` with return types from `src/types/`.

If anything is unclear or you want me to adapt the instructions for a different agent behavior (e.g., more conservative edits, tests first, or faster iterative PRs), tell me which style and I will update this file.
