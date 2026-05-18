# Smart Inventory — Front-End

Angular single-page application for the **Smart Inventory Management System**, branded for **Raya International Services**. It provides a role-based UI for home overview, dashboards, product catalog management, stock movements, inventory history, and warehouses (admin), backed by the [ASP.NET Core API](../Back-End/README.md).

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Configuration](#configuration)
- [Branding](#branding)
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
- [Troubleshooting](#troubleshooting)

---

## Features

| Area | Description |
|------|-------------|
| **Home** | Welcome screen, summary stat cards, and quick navigation links |
| **Dashboard** | Overview metrics, low-stock count, and low-stock product table |
| **Products** | Paginated list with search; create, edit, delete (admin) |
| **Warehouses** | Warehouse list view (admin only) |
| **Inventory** | Stock in / stock out dialogs; paginated transaction history with filters |
| **Authentication** | JWT login, route guards, session restore from `localStorage` |
| **Authorization** | Admin vs Employee UI; `adminGuard` on sensitive routes and sidebar items |
| **Layout** | `MainLayoutComponent` with collapsible `SidebarComponent`, `NavbarComponent`, and role-based menu |
| **Branding** | Raya International Services logo in sidebar, navbar, and login |
| **Theming** | Light / dark mode with preference persisted in `localStorage` (`sim-theme`) |
| **Loading UX** | Skeleton loaders, delayed global loading overlay, loading buttons |
| **Notifications** | Toast messages for success, errors, and HTTP failures |
| **Responsive layout** | Material side navigation shell with mobile-friendly navigation |

---

## Tech Stack

| Category | Technology |
|----------|------------|
| Framework | Angular 20 (standalone components, signals) |
| UI | Angular Material 20, SCSS |
| HTTP | `HttpClient` + functional interceptors |
| Authentication | JWT (`jwt-decode`), route guards |
| State | Angular signals, feature stores (no NgRx) |
| Notifications | `Toastr` 19 (Angular toast library) |
| Build | `@angular/build` (Angular application builder) |
| Container | Node 20 → Nginx 1.27 (production image) |

---

## Prerequisites

- [Node.js](https://nodejs.org/) **20.x** (LTS recommended)
- [`npm`](https://www.npmjs.com/) 10+
- Running **Smart Inventory API** ([Back-End setup](../Back-End/README.md))

---

## Quick Start

### 1. Install dependencies

```bash
cd smart-inventory-ui
npm install
```

### 2. Point the UI at your API

Default development API URL: `http://localhost:5166/api` (matches `dotnet run` HTTP profile in the Back-End).

Edit `public/env.js` if your API runs on another host or port:

```javascript
window.__env = {
  production: false,
  apiUrl: 'http://localhost:5166/api',
  appName: 'Raya International Services',
};
```

### 3. Start the API

From the Back-End folder:

```bash
dotnet run --project SmartInventorySystem.API --launch-profile http
```

API: **http://localhost:5166** · Swagger: **http://localhost:5166/swagger**

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

> Sign in with the seeded **username** and password (not an email address). After login you are redirected to **`/home`**.

---

## Configuration

### Runtime config (`public/env.js`)

Loaded before the Angular bundle via `index.html`. **Overrides** build-time values in `src/environments/` when `window.__env` is set.

| Key | Description |
|-----|-------------|
| `apiUrl` | Base URL for all API calls (must include `/api`) |
| `appName` | Display name used where environment title is referenced |
| `production` | `true` / `false` |

### Build-time defaults (`src/environments/`)

- `environment.ts` — production defaults (`apiUrl` typically `http://localhost:8080/api` for Docker)
- `environment.development.ts` — used when running `ng serve` (file replacement)

### Docker environment variables

| Variable | Default | Description |
|----------|---------|-------------|
| `API_URL` | `http://localhost:8080/api` | Browser-reachable API URL |
| `APP_NAME` | `Smart Inventory` | Injected into `env.js` as `appName` |
| `FRONTEND_PORT` | `8081` | Host port mapping |

---

## Branding

Centralized in `src/app/core/brand.config.ts`:

| Constant | Value |
|----------|--------|
| `logoUrl` | `/images/raya-logo.png` |
| `logoAlt` | `Raya International Services` |
| `companyName` | `Raya International Services` |
| `productName` | `Smart Inventory` |

Logo file: `public/images/raya-logo.png` (also used as the browser favicon via `index.html`).

Used in **sidebar**, **navbar**, and **login** pages. To replace the logo, swap the PNG under `public/images/` and keep the path in `brand.config.ts`.

---

## Project Structure

```
smart-inventory-ui/
├── public/
│   ├── images/
│   │   └── raya-logo.png      # Company logo
│   ├── env.js                 # Dev runtime config
│   └── env.js.template        # Docker startup template
├── src/
│   ├── app/
│   │   ├── core/              # Providers, HTTP tokens, loading, brand.config.ts
│   │   ├── routes/            # auth.routes.ts, main.routes.ts
│   │   ├── guards/            # authGuard, guestGuard, adminGuard
│   │   ├── interceptors/      # auth, loading, error
│   │   ├── layout/            # MainLayoutComponent, SidebarComponent, NavbarComponent
│   │   ├── pages/             # Routed page components (*.page.ts)
│   │   ├── features/          # Domain services, stores, dialogs
│   │   ├── shared/            # Reusable UI, utils, constants
│   │   ├── store/             # AsyncState, cache invalidation, pipeline helpers
│   │   ├── models/            # Shared TypeScript DTOs
│   │   └── theme/             # Material theme, ThemeService, CSS variables
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
| Routed views | `*.page.ts` | `home.page.ts`, `products-list.page.ts` |
| Layout shell | `*.component.ts` | `main-layout.ts` → `MainLayoutComponent` |
| Feature logic | `features/<domain>/` | `features/products/products.store.ts` |
| Shared UI | `shared/components/` | `page-header`, `data-table` |
| Selectors | `app-<name>-page` | `app-home-page` |

---

## Architecture

```
┌──────────────────────────────────────────────────────────────────┐
│  MainLayoutComponent (side nav + navbar + router-outlet)          │
│    └── Pages: home, dashboard, products, form, history, warehouses │
└───────────────────────────────┬──────────────────────────────────┘
                                │
┌───────────────────────────────▼──────────────────────────────────┐
│  Feature stores (signals)  ←  CacheInvalidationService            │
└───────────────────────────────┬──────────────────────────────────┘
                                │
┌───────────────────────────────▼──────────────────────────────────┐
│  Feature services (HTTP)  →  Interceptors  →  ASP.NET API         │
└──────────────────────────────────────────────────────────────────┘
```

- **Lazy loading**: `Auth layout and main layout load via` `loadComponent()`; each page is a separate chunk.
- **No NgRx**: Stores use `signal<AsyncState<T>>`, RxJS `Subject` triggers, and `connectAsyncStorePipeline()`.
- **Cache invalidation**: Mutations call `CacheInvalidationService` to refresh related stores.
- **HTTP interceptors** (order): `auth` → `loading` → `error`.
- **Animations**: `provideAnimations()` in `app.config.ts` (required for Material side navigation and toolbar).

---

## Routing

Authenticated routes render inside `MainLayoutComponent`. `/login` uses `AuthLayout` only (no sidebar).

| Path | Guard | Page | Lazy chunk |
|------|-------|------|------------|
| `/login` | `guestGuard` | Login | `login-page` |
| `/home` | `authGuard` | Home (default) | `home-page` |
| `/dashboard` | `authGuard` | Dashboard | `dashboard-page` |
| `/products` | `authGuard` | Product list | `products-list-page` |
| `/products/new` | `authGuard`, `adminGuard` | Create product | `product-form-page` |
| `/products/:id/edit` | `authGuard`, `adminGuard` | Edit product | `product-form-page` |
| `/inventory/history` | `authGuard` | Transaction history | `inventory-history-page` |
| `/warehouses` | `authGuard`, `adminGuard` | Warehouses list | `warehouses-list-page` |
| `**` | — | Redirect → `/home` | — |

Route definitions: `src/app/routes/auth.routes.ts`, `src/app/routes/main.routes.ts`, composed in `app.routes.ts`.

### Sidebar navigation (role-based)

| Menu item | Admin | Employee |
|-----------|:-----:|:--------:|
| Home | ✓ | ✓ |
| Dashboard | ✓ | ✓ |
| Products | ✓ | ✓ |
| Add Product | ✓ | — |
| Inventory History | ✓ | ✓ |
| Warehouses | ✓ | — |

Configured in `src/app/layout/navigation.config.ts` via `getNavItemsForRoles()`.

---

## Authentication & Roles

### Flow

1. `POST /api/auth/login` with credentials → JWT
2. Token stored in `localStorage` (`sim_access_token`)
3. `authInterceptor` attaches `Authorization: Bearer <token>`
4. On **401**, user is logged out and redirected to `/login`
5. Successful login navigates to `returnUrl` `query param` or **`/home`**

### Roles in the UI

| Role | Capabilities |
|------|----------------|
| **Admin** | Create/edit/delete products, stock in/out, warehouses view, all navigation items |
| **Employee** | View home, dashboard, products, and history; no product mutations or stock actions |

### Guards

| Guard | Purpose |
|-------|---------|
| `authGuard` | Requires valid JWT for main app routes |
| `guestGuard` | Login page only when logged out |
| `adminGuard` | Blocks non-admins from admin-only routes (redirects to `/home`) |

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
| `DashboardStore` | Stats and low-stock products (dashboard + home summary) |
| `ProductsStore` | Paginated product list + picker cache for dialogs |
| `InventoryHistoryStore` | Paginated transaction history |
| `WarehousesStore` | Warehouse list (warehouses page + product form) |

Helpers: `selectLoading`, `selectError`, `connectAsyncStorePipeline` in `src/app/store/`.

### Skipping the global loader

GET requests from data services use `dataRequestOptions()` (`SKIP_GLOBAL_LOADER`) so list pages show skeletons instead of the full-screen overlay.

---

## Shared UI Components

Exported from `src/app/shared/index.ts`:

| Component | Purpose |
|-----------|---------|
| `PageHeader` | Title, subtitle, action slot |
| `SearchField` | Search input with typing delay before filter runs, plus clear button |
| `DataTable` | Declarative columns + skeleton loading |
| `PaginatedTableShell` | Table wrapper + skeleton + paginator |
| `TableSkeleton` | Table loading placeholder |
| `StatCard` / `StatCardsSkeleton` | Metric cards |
| `FormSkeleton` | Form loading placeholder |
| `LoadingButton` | Material button with spinner |
| `ErrorState` | Error message + retry |
| `ConfirmDialog` | Delete confirmation |
| `GlobalLoader` | Delayed HTTP loading overlay (root `App` component) |

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
  -e APP_NAME="Raya International Services" \
  smartinventory-frontend
```

Production build uses a multi-stage **`Dockerfile`** (Node build → Nginx). `docker-entrypoint.sh` generates `/env.js` from environment variables at container start.

---

## Full Stack (Front-End + API)

From the repository root (`Smart Inventory Management System/`):

```powershell
cd "Smart Inventory Management System"
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
| API errors / CORS | API running; `apiUrl` in `public/env.js` matches API base URL (e.g. `http://localhost:5166/api` locally) |
| Login fails | Username/password from seed data; API reachable |
| Black / blank screen after login | Browser console for failed lazy chunks; confirm API responds; try light theme (navbar toggle); ensure `provideAnimations()` is enabled |
| Blank page after deploy | `env.js` generated at container start; Nginx serving `index.html` for SPA routes |
| Global loader stuck | Network tab for pending `/api` requests; token expiry (401) |
| Logo missing | File exists at `public/images/raya-logo.png`; hard-refresh cache |

---

Built with Angular 20 · Raya International Services · Smart Inventory Management System
