# RIS Task — Smart Inventory & POS System

Monorepo containing the full Smart Inventory Management System and POS Desktop application.

## Structure

```
RIS_Task/
├── backend/              .NET 8 API (Smart Inventory)
├── frontend/             Angular web client
├── POSDesktopSystem/     WinForms POS desktop app
├── Database/             SQL scripts (add your scripts here)
├── docker-compose.yml    Full stack (SQL Server + API + UI)
└── README.md
```

## Quick start — full stack (Docker)

```bash
docker compose up --build -d
```

| Service   | URL                        |
|-----------|----------------------------|
| Front-end | http://localhost:8081      |
| API       | http://localhost:8080      |
| SQL Server| localhost:1433             |

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

See [frontend/README.md](./frontend/README.md) and [frontend/smart-inventory-ui/README.md](./frontend/smart-inventory-ui/README.md).

### POS Desktop System

Open `POSDesktopSystem/POSDesktopSystem.sln` in Visual Studio.

See [POSDesktopSystem/README.md](./POSDesktopSystem/README.md).

## Database

Place SQL scripts in the `Database/` folder.
