# RIS Task — Smart Inventory & POS System

`Monorepo` containing the full Smart Inventory Management System and POS Desktop application.
##Users 
```
POS Window Application
-- cashier / cashier123
-- manager / manager123


Smart Inventory System
-- admin / Admin@123
-- employee / Employee@123

These users are seeded in the database 
```
## Structure

```
RIS_Task/
├── backend/              .NET 8 API (Smart Inventory)
├── frontend/             Angular web client
├── POSDesktopSystem/     WinForms POS desktop app
├── scripts/              start-all.ps1, stop-all.ps1
├── docker-compose.yml    Full stack (SQL Server + API + Web UI + POS DB)
└── README.md
```

## Quick start — run everything

**One command** starts Docker (SQL Server, API, Web UI, POS database) and opens the POS desktop app:

```powershell
.\scripts\start-all.ps1
```

Or double-click `start-all.bat`.

| Project | How it runs | URL / access |
|---------|-------------|--------------|
| Web UI | Docker | http://localhost:8081 |
| API | Docker | http://localhost:8080 |
| SQL Server | Docker | localhost:1433 |
| POS Desktop | Windows (WinForms) | Opens automatically |

**Stop Docker services:**

```powershell
.\scripts\stop-all.ps1
```

Close the POS desktop window manually when done.

### Docker only (no desktop app)

```bash
docker compose up --build -d
```

This starts SQL Server, API, Web UI, and runs POS database migrations. Launch the desktop app separately:

```powershell
$env:DOTNET_ENVIRONMENT = "Docker"
dotnet run --project POSDesktopSystem/src/POS.UI/POS.UI.csproj
```

## Individual projects

### Backend API

```bash
cd backend
dotnet run --project SmartInventorySystem.API
```

See [backend/README.md](./backend/README.md) for details.

### Frontend (Angular)

```bash
cd frontend/smart-inventory-ui
npm install
npm start
```

Open http://localhost:4200 (requires the API running).

See [frontend/README.md](./frontend/README.md) and the [Angular client README](./frontend/smart-inventory-ui/README.md).

### POS Desktop System

Open `POSDesktopSystem/POSDesktopSystem.sln` in Visual Studio, or use `start-all.ps1`.

See [POSDesktopSystem/README.md](./POSDesktopSystem/README.md).

## Database

One SQL Server container hosts both databases:

- `SmartInventoryDb` — web inventory API
- `POSDesktopSystem` — POS desktop app

Place additional SQL scripts in the `Database/` folder.

## Configuration

Copy `.env.example` to `.env` to override defaults (password, ports, etc.).
