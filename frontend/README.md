# Smart Inventory — Front-End Workspace

This folder contains the Angular web client for the Smart Inventory Management System.

## Application

All source code lives in:

**[`smart-inventory-ui/`](./smart-inventory-ui/)**

See the full application documentation:

**[smart-inventory-ui/README.md](./smart-inventory-ui/README.md)**

## Quick commands

```bash
cd smart-inventory-ui
npm install
npm start
```

Open http://localhost:4200 (requires the [Back-End API](../Back-End/README.md) running).

## Docker (UI only)

```bash
cd smart-inventory-ui
docker compose up --build -d
```

UI: http://localhost:8081

## Full stack (UI + API + SQL Server)

From the repository root:

```bash
docker compose -f Back-End/docker-compose.yml -f Front-End/docker-compose.frontend.yml up --build -d
```

| Service | URL |
|---------|-----|
| Front-end | http://localhost:8081 |
| API | http://localhost:8080 |
