# Smart Inventory System — Back-End API

ASP.NET Core 8 REST API for managing warehouses, products, inventory movements, and user access. Built with **Clean Architecture**, **Entity Framework Core**, **JWT authentication**, and optional **Docker** deployment.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Solution Structure](#solution-structure)
- [Prerequisites](#prerequisites)
- [Getting Started (Local)](#getting-started-local)
- [Configuration](#configuration)
- [Database & Migrations](#database--migrations)
- [Seed Data](#seed-data)
- [Authentication & Authorization](#authentication--authorization)
- [API Endpoints](#api-endpoints)
- [Docker](#docker)
- [Testing](#testing)
- [Project Conventions](#project-conventions)

---

## Features

- **Warehouses** — Create and list warehouse locations
- **Products** — Full CRUD with SKU uniqueness and warehouse assignment
- **Inventory** — Stock in / stock out with transaction history
- **JWT auth** — Custom lightweight authentication (no ASP.NET Identity)
- **Role-based access** — `Admin` and `Employee` policies
- **Pagination & search** — Product list and inventory history (search by product name)
- **In-memory caching** — Cached product and warehouse lists with automatic invalidation
- **Global exception handling** — Consistent JSON error responses
- **Swagger** — Interactive API documentation with JWT support
- **Auto migrations & seed** — Database ready on startup

---

## Tech Stack

| Category | Technology |
|----------|------------|
| Runtime | .NET 8 |
| API | ASP.NET Core Web API |
| ORM | Entity Framework Core 8 (SQL Server) |
| Auth | JWT Bearer + BCrypt password hashing |
| Validation | FluentValidation |
| Testing | xUnit, Moq, FluentAssertions |
| Containers | Docker, Docker Compose |

---

## Solution Structure

```
Back-End/
├── SmartInventorySystem.sln
├── SmartInventorySystem.API/              # Controllers, middleware, Swagger, Program.cs
├── SmartInventorySystem.Application/      # DTOs, interfaces, services, validators
├── SmartInventorySystem.Domain/           # Entities, enums, constants
├── SmartInventorySystem.Infrastructure/   # EF Core, repositories, JWT, cache, seed
├── SmartInventorySystem.Tests/            # Unit tests
├── Dockerfile
└── docker-compose.yml
```

### Dependency flow

```
API → Infrastructure → Application → Domain
API → Application
```

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server) or LocalDB (local development)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (optional, for containerized run)

---

## Getting Started (Local)

### 1. Clone and navigate

```bash
cd "Smart Inventory Management System/Back-End"
```

### 2. Update connection string

Edit `SmartInventorySystem.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SmartInventoryDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

### 3. Run the API

```bash
dotnet run --project SmartInventorySystem.API
```

On startup the application will:

1. Apply EF Core migrations
2. Seed sample data (if the database is empty)

### 4. Open Swagger

| Profile | URL |
|---------|-----|
| HTTP | http://localhost:5166/swagger |
| HTTPS | https://localhost:7151/swagger |

---

## Configuration

### `appsettings.json`

| Section | Description |
|---------|-------------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection |
| `Jwt:Issuer` | JWT issuer |
| `Jwt:Audience` | JWT audience |
| `Jwt:SecretKey` | Signing key (min. 32 characters) |
| `Jwt:ExpirationMinutes` | Token lifetime |
| `Cache:DefaultExpirationMinutes` | In-memory cache TTL |

### Environment variable overrides

Use double underscores for nested keys (e.g. in Docker):

```
ConnectionStrings__DefaultConnection=...
Jwt__SecretKey=...
Cache__DefaultExpirationMinutes=5
```

---

## Database & Migrations

Migrations are stored in:

`SmartInventorySystem.Infrastructure/Persistence/Migrations/`

### Apply migrations manually (optional)

```bash
dotnet ef database update \
  --project SmartInventorySystem.Infrastructure \
  --startup-project SmartInventorySystem.API
```

### Add a new migration

```bash
dotnet ef migrations add YourMigrationName \
  --project SmartInventorySystem.Infrastructure \
  --startup-project SmartInventorySystem.API \
  --output-dir Persistence/Migrations
```

---

## Seed Data

Seeding runs automatically on startup when the database has fewer than 10 products.

| Type | Count | Notes |
|------|-------|-------|
| Warehouses | 2 | Cairo & Alexandria |
| Products | 10 | Mixed IT, furniture, supplies |
| Users | 2 | Admin + Employee |
| Inventory transactions | 17 | Sample In/Out history |

### Default users

| Username | Password | Role |
|----------|----------|------|
| `admin` | `Admin@123` | Admin |
| `employee` | `Employee@123` | Employee |

> Change these credentials in production.

---

## Authentication & Authorization

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

Response includes a JWT `token`. Use it in Swagger (**Authorize**) or requests:

```
Authorization: Bearer {your-token}
```

### Roles & policies

| Role | Capabilities |
|------|------------|
| **Admin** | Full access — create/update/delete products, warehouses, inventory movements |
| **Employee** | Read-only — products, warehouses, inventory history |

| Policy | Roles |
|--------|-------|
| `AdminOnly` | Admin |
| `EmployeeOnly` | Employee |
| `AdminOrEmployee` | Admin, Employee |

---

## API Endpoints

### Auth

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/login` | Anonymous | Obtain JWT token |

### Products

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/products` | Admin, Employee | Paginated list (`pageNumber`, `pageSize`, `search`) |
| GET | `/api/products/{id}` | Admin, Employee | Get by ID |
| POST | `/api/products` | Admin | Create product |
| PUT | `/api/products/{id}` | Admin | Update product |
| DELETE | `/api/products/{id}` | Admin | Delete product |

### Warehouses

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/warehouses` | Admin, Employee | List all warehouses |
| POST | `/api/warehouses` | Admin | Create warehouse |

### Inventory

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/inventory/in` | Admin | Stock in (increase quantity) |
| POST | `/api/inventory/out` | Admin | Stock out (decrease quantity) |
| GET | `/api/inventory/history` | Admin, Employee | Paginated transaction history |

### Pagination query parameters

```
GET /api/products?pageNumber=1&pageSize=10&search=laptop
GET /api/inventory/history?pageNumber=1&pageSize=10&search=dell
```

### Example paged response

```json
{
  "items": [ ... ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 42,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

### Error response format

```json
{
  "statusCode": 400,
  "message": "Validation failed.",
  "errors": { "sku": ["SKU must be unique."] },
  "traceId": "00-..."
}
```

---

## Docker

### Requirements

- Docker Desktop with Compose v2

### Start SQL Server + API

```bash
cd Back-End
docker compose up --build -d
```

| Service | URL / Port |
|---------|------------|
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |
| SQL Server | `localhost,1433` |

### Environment variables (optional)

```bash
# PowerShell
$env:MSSQL_SA_PASSWORD="YourStrong@Passw0rd"
$env:JWT_SECRET_KEY="YourProductionSecretKey_AtLeast32Characters!"
docker compose up --build -d
```

### Useful commands

```bash
# View logs
docker compose logs -f api

# Stop containers
docker compose down

# Stop and remove database volume (fresh DB)
docker compose down -v

# Build image only
docker build -t smartinventory-api .
```

Docker startup applies migrations and seed data automatically.

---

## Testing

```bash
dotnet test SmartInventorySystem.Tests
```

Tests cover `ProductService` and `InventoryService` with mocked repositories (success, failure, and business rules).

---

## Project Conventions

- **Clean Architecture** — Domain has no dependencies; business rules live in Application services
- **Repository pattern** — Data access abstracted behind interfaces in Application
- **DTOs** — API contracts separated from domain entities
- **FluentValidation** — Request validation in Application layer
- **Async/await** — All I/O operations are asynchronous
- **No ASP.NET Identity** — Custom `ApplicationUser` with BCrypt + JWT

---

## Domain Model (summary)

| Entity | Key fields |
|--------|------------|
| `Warehouse` | Name, Location |
| `Product` | Name, SKU, Price, Quantity, WarehouseId |
| `InventoryTransaction` | ProductId, Quantity, TransactionType (In/Out), CreatedByUserId |
| `ApplicationUser` | Username, PasswordHash, Role |

---

## License

This project is part of the Smart Inventory Management System. Add your license here if applicable.
