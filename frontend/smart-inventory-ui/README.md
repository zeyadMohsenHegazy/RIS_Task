# Smart Inventory — Front-End

Angular single-page application for the **Smart Inventory Management System**. It provides a role-based UI for dashboards, product catalog management, stock movements, and inventory history, backed by the [ASP.NET Core API](../Back-End/README.md).

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Routing](#routing)
- [Authentication & Roles](#authentication--roles)
- [State Management](#state-management)
- [Shared UI Components](#shared-ui-components)
- [Docker](#docker)
- [Full Stack (Front-End + API)](#full-stack-front-end--api)
- [Scripts](#scripts)
- [Related Documentation](#related-documentation)

---

## Features

| Area | Description |
|------|-------------|
| **Dashboard** | Overview metrics, low-stock count, and low-stock product table |
| **Products** | Paginated list with search; create, edit, delete (admin) |
| **Inventory** | Stock in / stock out dialogs; paginated transaction history with filters |
| **Authentication** | JWT login, route guards, session restore from `localStorage` |
| **Authorization** | Admin vs Employee UI (read-only for employees on sensitive actions) |
| **Theming** | Light / dark mode with persisted preference |
| **Loading UX** | Skeleton loaders, debounced global overlay, loading buttons |
| **Notifications** | Toast messages for success, errors, and HTTP failures |
| **Responsive layout** | Material sidenav shell with mobile-friendly navigation |

---

## Tech Stack

| Category | Technology |
|----------|------------|
| Framework | Angular 20 (standalone components) |
| UI | Angular Material 20, SCSS |
| HTTP | `HttpClient` + functional interceptors |
| Auth | JWT (`jwt-decode`), route guards |
| State | Angular signals, feature stores (no NgRx) |
| Notifications | ngx-toastr 19 |
| Build | `@angular/build` (esbuild) |
| Container | Node 20 → Nginx 1.27 (production image) |

---

## Prerequisites

- [Node.js](https://nodejs.org/) **20.x** (LTS recommended)
- [npm](https://www.npmjs.com/) 10+
- Running **Smart Inventory API** ([Back-End setup](../Back-End/README.md))

---

## Quick Start

### 1. Install dependencies

```bash
cd smart-inventory-ui
npm install
```

### 2. Point the UI at your API

Default development API URL: `http://localhost:5000/api`

Edit `public/env.js` if your API runs elsewhere (e.g. `http://localhost:5166/api`):

```javascript
window.__env = {
  production: false,
  apiUrl: 'http://localhost:5166/api',
  appName: 'Smart Inventory (Dev)',
};
```

### 3. Start the API

From the Back-End folder:

```bash
dotnet run --project SmartInventorySystem.API
```

### 4. Start the dev server

```bash
npm start
# or: ng serve
```

Open **http://localhost:4200**

### 5. Sign in

Use the seeded API users (see [Back-End seed data](../Back-End/README.md#seed-data)):

| Username | Password | Role |
|----------|----------|------|
| `admin` | `Admin@123` | Admin |
| `employee` | `Employee@123` | Employee |

> The login screen labels the field **Email**; enter the API **username** value. The API expects `username` and `password` in the login request body.

---

## Configuration

### Runtime config (`public/env.js`)

Loaded before the Angular bundle via `index.html`. Used for local development and as the template source in Docker.

| Key | Description |
|-----|-------------|
| `apiUrl` | Base URL for all API calls (must include `/api`) |
| `appName` | Application title in the UI |
| `production` | `true` / `false` |

### Build-time defaults (`src/environments/`)

- `environment.ts` — production defaults (overridden by `window.__env` when present)
- `environment.development.ts` — used when running `ng serve` (with file replacement)

### Docker environment variables

| Variable | Default | Description |
|----------|---------|-------------|
| `API_URL` | `http://localhost:8080/api` | Browser-reachable API URL |
| `APP_NAME` | `Smart Inventory` | App title |
| `FRONTEND_PORT` | `8081` | Host port mapping |

---

## Project Structure

```
smart-inventory-ui/
├── public/
│   ├── env.js                 # Dev runtime config
│   └── env.js.template        # Docker startup template
├── src/
│   ├── app/
│   │   ├── core/              # Providers, HTTP tokens, loading, notifications
│   │   ├── routes/            # auth.routes.ts, main.routes.ts
│   │   ├── guards/            # authGuard, guestGuard
│   │   ├── interceptors/      # auth, loading, error
│   │   ├── layout/            # main-layout, sidebar, top-navbar
│   │   ├── pages/             # Routed page components (*.page.ts)
│   │   ├── features/          # Domain services, stores, dialogs
│   │   ├── shared/            # Reusable UI, utils, constants
│   │   ├── store/             # AsyncState, cache invalidation, pipeline helpers
│   │   ├── models/            # Shared TypeScript DTOs
│   │   └── theme/             # Material theme, CSS variables
│   ├── environments/
│   └── styles.scss
├── Dockerfile
├── nginx.conf
├── docker-compose.yml
└── angular.json
```

### Naming conventions

| Item | Convention | Example |
|------|------------|---------|
| Routed views | `*.page.ts` | `products-list.page.ts` |
| Feature logic | `features/<domain>/` | `features/products/products.store.ts` |
| Shared UI | `shared/components/` | `page-header`, `data-table` |
| Selectors | `app-<name>-page` | `app-products-list-page` |

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│  Pages (dashboard, products-list, product-form, history)     │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│  Feature stores (signals)  ←  CacheInvalidationService         │
└───────────────────────────┬─────────────────────────────────┘
                            │
┌───────────────────────────▼─────────────────────────────────┐
│  Feature services (HTTP)  →  Interceptors  →  ASP.NET API    │
└─────────────────────────────────────────────────────────────┘
```

- **Lazy loading**: All routes use standalone `loadComponent()` — each page is a separate bundle.
- **No NgRx**: Stores use `signal<AsyncState<T>>`, RxJS `Subject` triggers, and `connectAsyncStorePipeline()`.
- **Cache invalidation**: Mutations call `CacheInvalidationService` to refresh related stores.
- **HTTP interceptors** (order): `auth` → `loading` → `error`.

---

## Routing

| Path | Guard | Page | Lazy chunk |
|------|-------|------|------------|
| `/login` | guest | Login | `login-page` |
| `/dashboard` | auth | Dashboard | `dashboard-page` |
| `/products` | auth | Product list | `products-list-page` |
| `/products/new` | auth | Create product | `product-form-page` |
| `/products/:id/edit` | auth | Edit product | `product-form-page` |
| `/inventory/history` | auth | Transaction history | `inventory-history-page` |
| `**` | — | Redirect → `/dashboard` | — |

Route definitions: `src/app/routes/auth.routes.ts`, `src/app/routes/main.routes.ts`, composed in `app.routes.ts`.

---

## Authentication & Roles

### Flow

1. `POST /api/auth/login` with credentials → JWT
2. Token stored in `localStorage` (`sim_access_token`)
3. `authInterceptor` attaches `Authorization: Bearer <token>`
4. On **401**, user is logged out and redirected to `/login`

### Roles in the UI

| Role | Capabilities |
|------|----------------|
| **Admin** | Create/edit/delete products, stock in/out, all views |
| **Employee** | View products and history; no create/edit/delete or stock actions |

Guards: `authGuard` (protected routes), `guestGuard` (login only when logged out).

---

## State Management

### `AsyncState<T>`

```typescript
type LoadStatus = 'idle' | 'loading' | 'success' | 'error';

interface AsyncState<T> {
  status: LoadStatus;
  data: T | null;
  error: string | null;
}
```

### Feature stores

| Store | Responsibility |
|-------|----------------|
| `DashboardStore` | Stats and low-stock products |
| `ProductsStore` | Paginated product list + picker cache for dialogs |
| `InventoryHistoryStore` | Paginated transaction history |
| `WarehousesStore` | Warehouse list for product form |

Helpers: `selectLoading`, `selectError`, `connectAsyncStorePipeline` in `src/app/store/`.

### Skipping the global loader

GET requests from data services use `dataRequestOptions()` (`SKIP_GLOBAL_LOADER`) so list pages show skeletons instead of the full-screen overlay.

---

## Shared UI Components

Exported from `src/app/shared/index.ts`:

| Component | Purpose |
|-----------|---------|
| `PageHeader` | Title, subtitle, action slot |
| `SearchField` | Debounced search input with clear button |
| `DataTable` | Declarative columns + skeleton loading |
| `PaginatedTableShell` | Table wrapper + skeleton + paginator |
| `StatCard` / `StatCardsSkeleton` | Dashboard metrics |
| `FormSkeleton` | Form loading placeholder |
| `LoadingButton` | Material button with spinner |
| `ErrorState` | Error message + retry |
| `ConfirmDialog` | Delete confirmation |
| `GlobalLoader` | Debounced HTTP overlay (in `app.ts`) |

---

## Docker

### Front-end only

```bash
cd smart-inventory-ui
cp .env.example .env   # optional
docker compose up --build -d
```

| Service | URL |
|---------|-----|
| UI | http://localhost:8081 |

### Build image manually

```bash
docker build -t smartinventory-frontend .
docker run --rm -p 8081:80 \
  -e API_URL=http://localhost:8080/api \
  -e APP_NAME="Smart Inventory" \
  smartinventory-frontend
```

Production build uses a multi-stage **Dockerfile** (Node build → Nginx). `docker-entrypoint.sh` generates `/env.js` from environment variables at container start.

---

## Full Stack (Front-End + API)

From the repository root (parent of `Front-End` and `Back-End`):

```bash
docker compose -f Back-End/docker-compose.yml -f Front-End/docker-compose.frontend.yml up --build -d
```

| Service | URL |
|---------|-----|
| Front-end | http://localhost:8081 |
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |

Ensure `API_URL` in the front-end container points to a URL the **browser** can reach (`http://localhost:8080/api`, not an internal Docker hostname).

---

## Scripts

| Command | Description |
|---------|-------------|
| `npm start` | Dev server at http://localhost:4200 |
| `npm run build` | Production build → `dist/smart-inventory-ui` |
| `npm run watch` | Development build with watch mode |
| `npm test` | Unit tests (Karma + Jasmine) |

---

## Related Documentation

- [Back-End API README](../Back-End/README.md) — endpoints, database, Docker, seed data
- [Angular CLI](https://angular.dev/tools/cli) — generate components, build options

---

## Troubleshooting

| Issue | Check |
|-------|--------|
| API errors / CORS | API running; `apiUrl` in `public/env.js` matches API base URL |
| Login fails | Username/password from seed data; API reachable |
| Blank page after deploy | `env.js` generated; Nginx serving `index.html` for SPA routes |
| Global loader stuck | Network tab for failed requests; token expiry (401) |

---

Built with Angular 20 · Smart Inventory Management System
