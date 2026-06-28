# Task Management System (TMS)

Full-stack task management application built with Clean Architecture.

**Stack:** .NET 8 Web API · React + Vite · SQL Server · Docker

---

## Prerequisites

| Tool | Version | Purpose |
|---|---|---|
| [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) | 8.0+ | API build and run |
| [Node.js](https://nodejs.org/) | 20+ | Frontend build and dev server |
| [Docker Desktop](https://www.docker.com/products/docker-desktop/) | Latest | Containerised stack (Option B) |
| [Visual Studio 2022](https://visualstudio.microsoft.com/) | 17.8+ | Backend development (Option A) |
| [VS Code](https://code.visualstudio.com/) | Latest | Frontend development (Option A) |

---

## Option A — Local Development

### 1. Configure API secrets

```bash
cp src/TMS.API/appsettings.example.json src/TMS.API/appsettings.json
```

Edit `src/TMS.API/appsettings.json` and set your values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TmsDb;Trusted_Connection=True;"
  },
  "Jwt": {
    "Issuer": "TMS.API",
    "Audience": "TMS.Client",
    "Key": "YOUR_SECRET_KEY_MIN_32_CHARS_HERE"
  }
}
```

### 2. Apply the database migration

```bash
dotnet ef database update --project src/TMS.Infrastructure --startup-project src/TMS.API
```

### 3. Run the API

**Visual Studio 2022:** Open `TMS.sln`, set `TMS.API` as the startup project, press **F5**.

The API starts at `http://localhost:5212`. Swagger UI is available at `http://localhost:5212/swagger`.

**CLI alternative:**

```bash
dotnet run --project src/TMS.API
```

### 4. Run the frontend

Open the `frontend/` folder in VS Code, then in a terminal:

```bash
cd frontend
npm install
npm run dev
```

The React dev server starts at `http://localhost:5173`.

### Shortcut — run both together

From the `frontend/` directory:

```bash
npm run dev:all
```

This uses `concurrently` to start the .NET API and the Vite dev server simultaneously.

---

## Option B — Docker

### 1. Configure environment

```bash
cp .env.example .env
```

Edit `.env` and set a strong JWT key (minimum 32 characters):

```
SA_PASSWORD=YourStrong!Passw0rd
JWT_ISSUER=TMS.API
JWT_AUDIENCE=TMS.Client
JWT_KEY=YOUR_SECRET_KEY_MIN_32_CHARS_HERE
```

### 2. Build and start all services

```bash
docker compose up --build
```

| Service | URL |
|---|---|
| Frontend | http://localhost:3000 |
| API | http://localhost:5000 |
| Swagger UI | http://localhost:5000/swagger |
| SQL Server | localhost:1433 |

The API container automatically applies migrations and seeds data (`SEED_DATA=true`) on first start.

To stop:

```bash
docker compose down
```

To stop and remove the database volume:

```bash
docker compose down -v
```

---

## Environment Variables

| Variable | Description | Example |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | SQL Server connection string | `Server=sqlserver,1433;Database=TmsDb;...` |
| `Jwt__Key` | JWT signing key — minimum 32 characters, keep secret | `my-super-secret-key-min-32-chars!!` |
| `Jwt__Issuer` | JWT issuer claim | `TMS.API` |
| `Jwt__Audience` | JWT audience claim | `TMS.Client` |
| `SEED_DATA` | Set to `true` to insert sample tasks on startup | `true` |

> In Docker, double-underscore (`__`) maps to nested JSON keys (e.g. `Jwt__Key` → `Jwt.Key`).  
> For local dev, set these in `appsettings.json` (gitignored).

---

## Sample Login

```bash
curl -X POST http://localhost:5212/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"password123"}'
```

**Response:**

```json
{
  "success": true,
  "data": "<jwt-token>",
  "message": "Login successful.",
  "errors": [],
  "statusCode": 200
}
```

Use the token in subsequent requests:

```bash
curl http://localhost:5212/api/tasks \
  -H "Authorization: Bearer <jwt-token>"
```

---

## Running Tests

```bash
dotnet test tests/TMS.Tests
```

Expected output:

```
Total tests: 19
     Passed: 19
      Failed: 0
```

To run with detailed output:

```bash
dotnet test tests/TMS.Tests --logger "console;verbosity=normal"
```

---

## Project Structure

```
TMS.sln
├── src/
│   ├── TMS.Shared/          # ApiResponse<T> envelope (netstandard2.0)
│   ├── TMS.Domain/          # Entities, enums, NotFoundException
│   ├── TMS.Application/     # Interfaces, services, DTOs, validators
│   ├── TMS.Infrastructure/  # EF Core DbContext, migrations, seed, raw SQL
│   └── TMS.API/             # Controllers, middleware, Program.cs
├── tests/
│   └── TMS.Tests/           # xUnit unit tests
└── frontend/                # React + Vite + TypeScript
```

## Architecture

Clean Architecture with four layers and a shared envelope project:

```
TMS.Shared
    ↑
TMS.Domain
    ↑
TMS.Application ────────────────────┐
    ↑                               │
TMS.Infrastructure               TMS.API
```

No circular references. `TMS.Infrastructure` and `TMS.API` depend inward on `TMS.Application`; `TMS.Application` depends only on `TMS.Domain` and `TMS.Shared`.
