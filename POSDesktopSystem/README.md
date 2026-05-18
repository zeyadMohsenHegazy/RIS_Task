# POS Desktop System

A production-style **Point of Sale (POS)** desktop application built with **.NET 8**, **WinForms**, and **Clean Architecture**. Designed for retail inventory management, sales checkout, invoice history, receipt printing, PDF export, and optional offline SQLite mode with background sync.

---

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Docker](#docker)
- [Default Users](#default-users)
- [User Roles & Permissions](#user-roles--permissions)
- [Logging](#logging)
- [Project Structure](#project-structure)
- [Scripts](#scripts)
- [EF Core Migrations](#ef-core-migrations)
- [Manual SQL Scripts (Legacy)](#manual-sql-scripts-legacy)
- [Feature Guide](#feature-guide)
- [Troubleshooting](#troubleshooting)

---

## Features

| Area | Description |
|------|-------------|
| **Authentication** | Username/password login with SHA-256 hashing and role-based access |
| **Point of Sale** | Cart, barcode/name lookup, discounts, tax, cash/card checkout |
| **Barcode Support** | Case-insensitive lookup, scanner auto-add (Enter), multi-match product picker |
| **Product Management** | CRUD, search, stock tracking (Manager only) |
| **Invoice History** | Search by invoice # or cashier, detail view (Manager only) |
| **Receipt Printing** | Print preview and print — A4 or thermal (80 mm) formats |
| **PDF Export** | Export invoice details to PDF from invoice detail form |
| **Logging** | File-based logging for login, checkout, errors, and sync events |
| **SQLite Offline Mode** | Optional local SQLite database for offline operation |
| **Background Sync** | Sync pending invoices from SQLite to remote SQL Server via EF Core |
| **Docker** | Containerized SQL Server + EF Core migration container for dev/CI |

---

## Architecture

The solution follows **Clean Layered Architecture**. Dependencies point inward — the UI never uses ADO.NET or raw SQL; all data access goes through **EF Core**.

```
┌─────────────────────────────────────────────────────────┐
│  POS.UI (WinForms)                                      │
│  Forms, Views, Printing, PDF Export, DI CompositionRoot │
└──────────────────────────┬──────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────┐
│  POS.Application                                          │
│  Services, DTOs, Validators, Interfaces                 │
└──────────────────────────┬──────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────┐
│  POS.Infrastructure                                     │
│  EF Core DbContext, Repositories, Sync, File Logging    │
└──────────────────────────┬──────────────────────────────┘
                           │
┌──────────────────────────▼──────────────────────────────┐
│  POS.Domain                                             │
│  Entities, Enums                                        │
└─────────────────────────────────────────────────────────┘
```

### Key Patterns

- **Repository + Unit of Work** — data access abstracted behind interfaces
- **Dependency Injection** — `Microsoft.Extensions.DependencyInjection` with manual composition root
- **Service Layer** — business logic in Application services (`AuthService`, `PosService`, `InvoiceService`, etc.)
- **DTOs** — UI and services communicate via data transfer objects, not domain entities
- **EF Core Migrations** — schema managed by migrations (no runtime ADO.NET)
- **Atomic Checkout** — invoice staging + stock update in a single `SaveChanges` transaction

---

## Technology Stack

| Component | Technology |
|-----------|------------|
| Runtime | .NET 8 |
| UI | Windows Forms (`net8.0-windows`) |
| ORM | Entity Framework Core 8 |
| Databases | SQL Server, SQLite |
| DI / Config | Microsoft.Extensions.DependencyInjection, Configuration |
| PDF | QuestPDF (Community license) |
| Printing | System.Drawing.Printing (GDI+) |
| Container | Docker Compose (SQL Server 2022) |

---

## Prerequisites

- **Windows 10/11** (required for WinForms desktop UI)
- **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
- **SQL Server** (local install **or** Docker — see [Docker](#docker))
- **[Docker Desktop](https://www.docker.com/products/docker-desktop/)** (optional, for containerized SQL Server)
- **Visual Studio 2022** or **VS Code** (optional, for development)

---

## Quick Start

### Option A — Local SQL Server (Windows Authentication)

1. **Clone / open** the project folder containing `POSDesktopSystem.sln`.

2. **Update connection string** in `src/POS.UI/appsettings.json` if your SQL Server instance differs:
   ```json
   "DefaultConnection": "Server=localhost;Database=POSDesktopSystem;Trusted_Connection=True;TrustServerCertificate=True;"
   ```

3. **Build and run:**
   ```powershell
   dotnet build POSDesktopSystem.sln
   dotnet run --project src/POS.UI/POS.UI.csproj
   ```

4. On first login, the app **automatically applies EF Core migrations** and seeds default users.

### Option B — Docker SQL Server

See the [Docker](#docker) section, then run:

```powershell
.\scripts\run-with-docker.ps1
```

### Option C — SQLite Offline Mode

Set in `src/POS.UI/appsettings.json`:

```json
"Database": {
  "Provider": "Sqlite",
  "SqliteFileName": "pos-offline.db",
  "AutoMigrate": true,
  "SeedDefaultUsers": true
}
```

The SQLite file is created at `{AppFolder}/Data/pos-offline.db`.

---

## Configuration

Configuration files live in `src/POS.UI/` and are copied to the output directory on build.

| File | Purpose |
|------|---------|
| `appsettings.json` | Default settings (local SQL Server) |
| `appsettings.Docker.json` | Overrides when `DOTNET_ENVIRONMENT=Docker` |

### Full Configuration Reference

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "<primary database connection string>",
    "RemoteConnection": "<remote SQL Server for offline sync>"
  },
  "Database": {
    "Provider": "SqlServer",
    "SqliteFileName": "pos-offline.db",
    "AutoMigrate": true,
    "SeedDefaultUsers": true
  },
  "Sync": {
    "Enabled": false,
    "IntervalSeconds": 60
  },
  "Logging": {
    "LogDirectory": "Logs",
    "FileNamePrefix": "pos"
  }
}
```

| Setting | Values | Description |
|---------|--------|-------------|
| `Database:Provider` | `SqlServer`, `Sqlite` | Database provider (EF Core) |
| `Database:AutoMigrate` | `true` / `false` | Apply EF migrations on startup |
| `Database:SeedDefaultUsers` | `true` / `false` | Seed cashier/manager if no users exist |
| `Sync:Enabled` | `true` / `false` | Background sync (SQLite → remote SQL Server) |
| `Sync:IntervalSeconds` | number | Sync polling interval (minimum effective: 15s) |

### Environment Variables

The app loads environment variables after JSON config (later sources override earlier ones).

| Variable | Example | Description |
|----------|---------|-------------|
| `DOTNET_ENVIRONMENT` | `Docker` | Loads `appsettings.Docker.json` |
| `ConnectionStrings__DefaultConnection` | `Server=...` | Override connection string |
| `Database__Provider` | `Sqlite` | Override database provider |
| `Sync__Enabled` | `true` | Enable background sync |

---

## Database Setup

### Automatic (Recommended)

With `Database:AutoMigrate: true` (default), the app runs on first login:

1. `DatabaseInitializer.InitializeAsync()` → `context.Database.MigrateAsync()`
2. Seeds default users if `SeedDefaultUsers` is true and the Users table is empty

No manual SQL scripts are required for new installations.

### Existing Database from Manual Scripts

If you previously created the database using scripts in `database/Scripts/`, set:

```json
"Database": { "AutoMigrate": false }
```

Or baseline the `__EFMigrationsHistory` table before enabling migrations.

### Offline + Sync Configuration

For SQLite locally with sync to a remote SQL Server:

```json
{
  "ConnectionStrings": {
    "RemoteConnection": "Server=localhost;Database=POSDesktopSystem;User Id=sa;Password=...;TrustServerCertificate=True;"
  },
  "Database": {
    "Provider": "Sqlite"
  },
  "Sync": {
    "Enabled": true,
    "IntervalSeconds": 60
  }
}
```

- Invoices created offline are marked **Pending** sync.
- **Background sync** pushes them to the remote database (matched by username and product barcode).
- Managers see a **Sync Now** button and sync status in invoice history when offline mode is active.

---

## Docker

The WinForms UI runs on **Windows**; Docker hosts **SQL Server** and an optional **EF Core migration** container.

### Architecture

```
┌─────────────────────┐      localhost:1433       ┌──────────────────────┐
│  POS.UI (WinForms)  │ ────────────────────────► │  Docker: sqlserver   │
│  runs on Windows    │     SQL auth (sa/password) │  SQL Server 2022     │
└─────────────────────┘                             └──────────────────────┘
         │                                                    ▲
         │ EF Core MigrateAsync on login                      │
         └────────────────────────────────────────────────────┘

Optional: db-init container runs POS.DbMigrator (EF Core migrations + seed)
```

### Setup

1. **Create `.env` from template:**
   ```powershell
   copy .env.example .env
   ```

2. **Ensure passwords match** — `.env` `MSSQL_SA_PASSWORD` must match `appsettings.Docker.json`.

3. **Start SQL Server:**
   ```powershell
   docker compose up -d sqlserver
   ```

4. **(Optional) Run migrations in Docker:**
   ```powershell
   docker compose up db-init
   ```

5. **Run the desktop app:**
   ```powershell
   $env:DOTNET_ENVIRONMENT = "Docker"
   dotnet run --project src/POS.UI/POS.UI.csproj
   ```

   Or use the helper script:
   ```powershell
   .\scripts\run-with-docker.ps1
   ```

### Docker Commands

| Command | Description |
|---------|-------------|
| `docker compose up -d` | Start SQL Server in background |
| `docker compose up db-init` | Run EF Core migrations + seed once |
| `docker compose logs -f sqlserver` | View SQL Server logs |
| `docker compose down` | Stop containers |
| `docker compose down -v` | Stop containers and delete data volume |

### Docker Files

| File | Purpose |
|------|---------|
| `docker-compose.yml` | SQL Server + `db-init` services |
| `docker/Dockerfile.migrate` | Builds `POS.DbMigrator` image |
| `.env.example` | Environment variable template |
| `.dockerignore` | Excludes build artifacts from Docker context |

### POS.DbMigrator

Console project (`src/POS.DbMigrator`) used by the `db-init` Docker service. It calls the same `DatabaseInitializer` as the desktop app — **EF Core only**, no ADO.NET.

```powershell
dotnet run --project src/POS.DbMigrator/POS.DbMigrator.csproj
```

Pass connection string via environment variable:
```powershell
$env:ConnectionStrings__DefaultConnection = "Server=localhost,1433;Database=POSDesktopSystem;User Id=sa;Password=Your_strong_password123!;TrustServerCertificate=True;"
dotnet run --project src/POS.DbMigrator/POS.DbMigrator.csproj
```

---

## Default Users

Seeded automatically when `SeedDefaultUsers` is true (EF Core seed, same hashes as legacy SQL script):

| Username | Password | Role |
|----------|----------|------|
| `cashier` | `cashier123` | Cashier |
| `manager` | `manager123` | Manager |

Passwords are stored as **SHA-256** hashes in the database.

---

## User Roles & Permissions

| Feature | Cashier | Manager |
|---------|:-------:|:-------:|
| Point of Sale (checkout) | ✅ | ✅ |
| Product Management | ❌ | ✅ |
| Invoice History | ❌ | ✅ |
| Manual Sync (offline mode) | ❌ | ✅ |

Role logic is enforced in `RolePermissions` (Application layer) and reflected in dashboard navigation.

---

## Logging

Logs are written to `{AppBaseDirectory}/Logs/pos-{yyyy-MM-dd}.log`.

| Level | Usage |
|-------|-------|
| `INFO` | App startup, sync status |
| `WARN` | Non-critical issues |
| `ERROR` | Exceptions with stack traces |
| `ACTION` | Login, logout, invoice creation, sync events |

### Logged Events

- Login attempts (success/failure)
- Logout
- Invoice creation
- Unhandled UI exceptions
- UI operation errors
- Background and manual sync results

Configure in `appsettings.json`:

```json
"Logging": {
  "LogDirectory": "Logs",
  "FileNamePrefix": "pos"
}
```

---

## Project Structure

```
POSDesktopSystem/
├── POSDesktopSystem.sln
├── docker-compose.yml
├── .env.example
├── .dockerignore
├── README.md
├── docker/
│   └── Dockerfile.migrate
├── scripts/
│   ├── start-docker-db.ps1
│   └── run-with-docker.ps1
├── database/
│   └── Scripts/              # Legacy manual SQL (optional)
│       ├── CreateDatabase.sql
│       ├── SeedUsers.sql
│       └── MigrateInvoicesAddUser.sql
└── src/
    ├── POS.Domain/             # Entities, Enums
    ├── POS.Application/        # Services, DTOs, Validators, Interfaces
    ├── POS.Infrastructure/     # EF Core, Repositories, Sync, Logging
    ├── POS.DbMigrator/         # Console EF migration runner (Docker/CI)
    └── POS.UI/                 # WinForms app, Views, Printing, Export
        ├── appsettings.json
        ├── appsettings.Docker.json
        ├── Forms/              # LoginForm, DashboardForm, ProductForm, etc.
        ├── Views/                # PosView, ProductsView, InvoiceHistoryView
        ├── Helpers/              # UiTheme, DataGridHelper, ErrorDialog, etc.
        ├── Printing/             # Receipt printer (A4 / Thermal)
        └── Export/               # Invoice PDF exporter (QuestPDF)
```

### Solution Projects

| Project | Type | Description |
|---------|------|-------------|
| `POS.Domain` | Class Library | Domain entities and `enums` |
| `POS.Application` | Class Library | Business logic, DTOs, validators |
| `POS.Infrastructure` | Class Library | EF Core, repositories, sync, file logging |
| `POS.UI` | WinExe | WinForms desktop application |
| `POS.DbMigrator` | Console | EF Core migration + seed runner |

---

## Scripts

| Script | Description |
|--------|-------------|
| `scripts/start-docker-db.ps1` | Creates `.env` if missing, starts SQL Server container, waits for health |
| `scripts/run-with-docker.ps1` | Starts Docker DB + runs POS app with `DOTNET_ENVIRONMENT=Docker` |

---

## EF Core Migrations

Migrations live in `src/POS.Infrastructure/Data/Migrations/`.

### Apply migrations (CLI)

```powershell
dotnet ef database update `
  --project src/POS.Infrastructure/POS.Infrastructure.csproj `
  --startup-project src/POS.Infrastructure/POS.Infrastructure.csproj
```

### Add a new migration

```powershell
dotnet ef migrations add MigrationName `
  --project src/POS.Infrastructure/POS.Infrastructure.csproj `
  --startup-project src/POS.Infrastructure/POS.Infrastructure.csproj `
  --output-dir Data/Migrations
```

### Design-time factory

`POSDbContextFactory` in Infrastructure provides design-time DbContext for EF tools.

---

## Manual SQL Scripts (Legacy)

The `database/Scripts/` folder contains optional **manual** setup scripts from early development. **The app does not execute these at runtime** — EF Core migrations are the supported path.

| Script | Purpose |
|--------|---------|
| `CreateDatabase.sql` | Manual database/table creation |
| `SeedUsers.sql` | Manual user seed (same users as EF seed) |
| `MigrateInvoicesAddUser.sql` | Migration helper for existing DBs |

Use only if you cannot use EF migrations (`AutoMigrate: false`).

---

## Feature Guide

### Point of Sale

1. Log in as **cashier** or **manager**.
2. Scan or type a **barcode** and press **Enter** — exact match auto-adds qty 1 to cart.
3. Type a **product name** to search; pick from list if multiple matches.
4. Set quantity, apply discount/tax, pay **Cash** or **Card**.
5. Optionally print receipt after checkout.

### Product Management (Manager)

- Add, edit, delete products with validation (unique barcode, positive price/stock).
- `Debounced` search grid.

### Invoice History (Manager)

- Search by invoice number or cashier name.
- View details, print, or **Export PDF**.
- Sync status column visible in offline mode.

### Receipt Printing

- **A4** — full page layout.
- **Thermal (80 mm)** — compact receipt for thermal printers.
- Available from invoice detail and post-checkout prompt.

### PDF Export

- Open invoice detail → **Export PDF** → choose save location.
- Generated via QuestPDF with invoice header, line items, and totals.

### Barcode Scanner

- USB barcode scanners typically act as keyboard input.
- Focus the barcode field on POS screen; scan sends digits + Enter.
- Exact barcode match adds product to cart immediately.

---

## Troubleshooting

### Database initialization failed on login

- Verify SQL Server is running and the connection string is correct.
- For Docker: ensure container is healthy (`docker ps`) and password matches `.env` / `appsettings.Docker.json`.
- Check firewall allows port **1433**.

### Docker health check fails

- Confirm `MSSQL_SA_PASSWORD` meets complexity requirements (upper, lower, number, symbol, 8+ characters).
- Allow 30–60 seconds for first startup.
- View logs: `docker compose logs sqlserver`

### Migration errors on existing database

- Tables already exist from manual scripts → set `"AutoMigrate": false` or baseline `__EFMigrationsHistory`.

### POS.UI.exe locked during build

- Close the running POS application before rebuilding.

### Sync failures (offline mode)

- Remote SQL Server must have matching **users** (by username) and **products** (by barcode).
- Check invoice sync status and error message in invoice detail.
- Use **Sync Now** on the dashboard (Manager, offline mode).

### Logs location

```
src/POS.UI/bin/Debug/net8.0-windows/Logs/pos-{date}.log
```

---

## Build

```powershell
dotnet restore POSDesktopSystem.sln
dotnet build POSDesktopSystem.sln
dotnet run --project src/POS.UI/POS.UI.csproj
```

---

## License

This project is intended for educational and portfolio use. QuestPDF uses the [Community license](https://www.questpdf.com/license/) — verify compliance for commercial deployment.
