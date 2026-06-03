# TradieFlow

A full-stack client & job management app for small trade businesses — built to demonstrate a complete, production-style slice: a layered .NET API, a PostgreSQL database, a typed React frontend, automated tests, and continuous integration.

> Manage clients, track their jobs through a clear status workflow (Quoted → Scheduled → In Progress → Completed), and keep everything backed by a real relational database.

---

## Tech stack

| Layer        | Technology |
|--------------|------------|
| Backend      | .NET 9, ASP.NET Core Web API (controllers) |
| Data access  | Entity Framework Core 9 |
| Database     | PostgreSQL 16 (via Docker) |
| Frontend     | React 19, TypeScript, Vite |
| Testing      | xUnit, EF Core InMemory, **Testcontainers**, `WebApplicationFactory` |
| CI           | GitHub Actions |

---

## Architecture

The backend follows a layered (Clean Architecture) structure, where dependencies point **inward** — the domain knows nothing about the database or the web.

```
TradieFlow.Api            ──▶  controllers, HTTP, DI, config
   │
   ├──▶ TradieFlow.Application   ──▶  DTOs, service interfaces (use cases)
   │
   └──▶ TradieFlow.Infrastructure ──▶  EF Core DbContext, service implementations, migrations
            │
            └──▶ TradieFlow.Domain  ──▶  entities & enums (no dependencies)
```

- **Domain** — plain entities (`Client`, `Job`) and the `JobStatus` enum. No framework dependencies.
- **Application** — request/response DTOs and service interfaces (the use cases).
- **Infrastructure** — EF Core `DbContext`, service implementations, and database migrations.
- **Api** — thin controllers that delegate to the services, plus configuration and dependency injection.

The React client lives in `client/` and talks to the API over HTTP (CORS-enabled for local development).

---

## Features

- **Client management** — full CRUD with server-side validation.
- **Job management** — each job belongs to a client (a one-to-many relationship enforced by a database foreign key). List a client's jobs, create jobs, update their status, and delete them.
- **Status workflow** — jobs move through `Quoted`, `Scheduled`, `InProgress`, `Completed`, `Cancelled`, stored as readable text in the database.
- **Validation** — invalid input is rejected with `400 Bad Request` and standardized [RFC 9110 Problem Details](https://www.rfc-editor.org/rfc/rfc9110), including field-level error messages.
- **Correct REST semantics** — `201 Created` with a `Location` header on create, `204 No Content` on update/delete, `404 Not Found` for missing resources.

---

## Getting started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 22+
- [Docker](https://www.docker.com/products/docker-desktop/) (for PostgreSQL and the integration tests)

### 1. Start the database

```bash
docker compose up -d
```

### 2. Apply migrations and run the API

```bash
# install the EF Core CLI once, if you don't have it
dotnet tool install --global dotnet-ef

# create the database schema
dotnet ef database update --project src/TradieFlow.Infrastructure --startup-project src/TradieFlow.Api

# run the API (http://localhost:5167)
dotnet run --project src/TradieFlow.Api
```

### 3. Run the frontend

```bash
cd client
npm install
npm run dev          # http://localhost:5173
```

Open http://localhost:5173 and you're up.

---

## Testing

```bash
dotnet test          # Docker must be running for the integration tests
```

The suite has two layers:

- **Unit tests** — exercise the service logic in isolation using the EF Core InMemory provider.
- **Integration tests** — use `WebApplicationFactory` to boot the entire API in memory and **Testcontainers** to spin up a real, throwaway PostgreSQL container, then drive the API over real HTTP. This verifies the full stack — routing, validation, services, EF Core, real SQL, and the migrations — exactly as it runs in production.

The same tests run automatically in CI on every push.

---

## Continuous integration

Every push and pull request to `main` triggers [GitHub Actions](.github/workflows/ci.yml), which:

1. Builds the .NET solution and runs the full test suite (including the Testcontainers integration tests — the CI runner has Docker available).
2. Installs frontend dependencies and builds the React app (type-checking the TypeScript).

---

## Project structure

```
TradieFlow/
├── src/
│   ├── TradieFlow.Domain/          # entities, enums (no dependencies)
│   ├── TradieFlow.Application/     # DTOs, service interfaces
│   ├── TradieFlow.Infrastructure/  # EF Core DbContext, services, migrations
│   └── TradieFlow.Api/             # controllers, Program.cs, configuration
├── client/                         # React + TypeScript (Vite) frontend
├── tests/
│   └── TradieFlow.Tests/           # xUnit unit + integration tests
├── docker-compose.yml              # PostgreSQL for local development
└── .github/workflows/ci.yml        # GitHub Actions CI pipeline
```

---

## Roadmap

Planned next steps that extend the same vertical-slice pattern:

- **Invoices & line items** — generate invoices from completed jobs, with GST handling for the Australian market.
- **Authentication** — protect the API and scope data per user.
- **A hosted live demo** — deploy the API, database, and frontend.

---

## Author

Built by Zoe Wang — [codewithzoe.com](https://codewithzoe.com) · [github.com/zoewang66](https://github.com/zoewang66)
