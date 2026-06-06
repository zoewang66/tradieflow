# TradieFlow

A full-stack client, job, and invoice manager for small trade businesses. It is built as a complete, production-style slice: a layered .NET API, a managed PostgreSQL database, a typed React frontend, automated tests, continuous integration, and a hosted live deployment.

> Manage clients, track their jobs through a clear status workflow (Quoted, Scheduled, In Progress, Completed), and raise invoices with line items and Australian GST calculated on the server. Everything is backed by a real relational database.

[![CI](https://github.com/zoewang66/tradieflow/actions/workflows/ci.yml/badge.svg)](https://github.com/zoewang66/tradieflow/actions/workflows/ci.yml)

---

## Live demo

- **App:** https://tradieflow.codewithzoe.com
- **API:** https://tradieflow-api1.onrender.com

The demo opens with seeded sample data so there is something to explore straight away.

> Heads up: the demo API runs on a free tier, so the first request after a period of inactivity can take around 30 to 50 seconds to wake up. After that it is responsive.

---

## Tech stack

| Layer        | Technology |
|--------------|------------|
| Backend      | .NET 9, ASP.NET Core Web API (controllers) |
| Data access  | Entity Framework Core 9 |
| Database     | PostgreSQL 16 |
| Frontend     | React 19, TypeScript, Vite |
| Testing      | xUnit, EF Core InMemory, **Testcontainers**, `WebApplicationFactory` |
| CI           | GitHub Actions |
| Deployment   | Docker, Render (API), Neon (PostgreSQL), AWS S3 + CloudFront (frontend) |

---

## Architecture

The backend follows a layered (Clean Architecture) structure, where dependencies point **inward**: the domain knows nothing about the database or the web.

```
TradieFlow.Api            ──▶  controllers, HTTP, DI, config
   │
   ├──▶ TradieFlow.Application    ──▶  DTOs, service interfaces (use cases)
   │
   └──▶ TradieFlow.Infrastructure ──▶  EF Core DbContext, service implementations, migrations
            │
            └──▶ TradieFlow.Domain ──▶  entities, enums, and the GST / invoice calculator (no dependencies)
```

- **Domain:** plain entities (`Client`, `Job`, `Invoice`, `LineItem`), the `JobStatus` and `InvoiceStatus` enums, and the invoice and GST calculator. No framework dependencies.
- **Application:** request and response DTOs and service interfaces (the use cases).
- **Infrastructure:** EF Core `DbContext`, service implementations, database migrations, and seed data.
- **Api:** thin controllers that delegate to the services, plus configuration and dependency injection.

The React client lives in `client/` and talks to the API over HTTPS. The API base URL is supplied at build time so the same code runs locally and against the deployed API.

I deliberately kept this a single deployable service rather than splitting it into microservices. For a focused app that is the right level of complexity, and it is far easier to operate.

---

## Features

- **Client management:** full CRUD with server-side validation.
- **Job management:** each job belongs to a client (a one-to-many relationship enforced by a database foreign key). List a client's jobs, create jobs, update their status, and delete them.
- **Status workflow:** jobs move through `Quoted`, `Scheduled`, `InProgress`, `Completed`, `Cancelled`, stored as readable text in the database.
- **Invoicing with GST:** raise an invoice against a job with one or more line items. The subtotal, 10 percent Australian GST, and total are calculated and validated on the server, not in the browser. Each invoice gets a sequential number (for example `INV-00001`) and moves through `Draft`, `Sent`, `Paid`, `Cancelled`.
- **Validation:** invalid input is rejected with `400 Bad Request` and standardized [RFC 9110 Problem Details](https://www.rfc-editor.org/rfc/rfc9110), including field-level error messages.
- **Correct REST semantics:** `201 Created` with a `Location` header on create, `204 No Content` on update and delete, `404 Not Found` for missing resources.
- **Seeded demo data:** on first run the database is seeded with a sample client, job, and invoice, so the live demo always opens with content.

---

## Deployment

The app runs as a hosted live demo with each tier on a service suited to it:

- **Frontend:** built with Vite and served as static files from AWS S3 behind CloudFront, over HTTPS on a custom domain (https://tradieflow.codewithzoe.com).
- **API:** packaged as a Docker image (multi-stage build) and deployed to Render (https://tradieflow-api1.onrender.com).
- **Database:** managed PostgreSQL on Neon, connected over SSL.

On startup the API applies any pending EF Core migrations and seeds sample data if the database is empty. Configuration (database connection, allowed frontend origin, listening port) is read from environment variables, so the same container image runs locally and in production with no code changes.

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

Open http://localhost:5173 and you are up. The dev frontend talks to the API on `http://localhost:5167` by default.

To build the frontend against a different API, set `VITE_API_URL` at build time:

```bash
VITE_API_URL=https://tradieflow-api1.onrender.com npm run build
```

---

## Testing

```bash
dotnet test          # Docker must be running for the integration tests
```

The suite has two layers:

- **Unit tests:** exercise the service logic and the GST / invoice calculator in isolation, the calculator with plain xUnit and the services using the EF Core InMemory provider.
- **Integration tests:** use `WebApplicationFactory` to boot the entire API in memory and **Testcontainers** to spin up a real, throwaway PostgreSQL container, then drive the API over real HTTP. This verifies the full stack, routing, validation, services, EF Core, real SQL, and the migrations, exactly as it runs in production.

The same tests run automatically in CI on every push.

---

## Continuous integration

Every push and pull request to `main` triggers [GitHub Actions](.github/workflows/ci.yml), which:

1. Builds the .NET solution and runs the full test suite (including the Testcontainers integration tests, since the CI runner has Docker available).
2. Installs frontend dependencies and builds the React app (type-checking the TypeScript).

---

## Project structure

```
TradieFlow/
├── src/
│   ├── TradieFlow.Domain/          # entities, enums, InvoiceCalculator (no dependencies)
│   ├── TradieFlow.Application/     # DTOs, service interfaces
│   ├── TradieFlow.Infrastructure/  # EF Core DbContext, services, migrations, seed data
│   └── TradieFlow.Api/             # controllers, Program.cs, configuration
├── client/                         # React + TypeScript (Vite) frontend
├── tests/
│   └── TradieFlow.Tests/           # xUnit unit + integration tests
├── docker-compose.yml              # PostgreSQL for local development
├── Dockerfile                      # multi-stage build for the API container (deployment)
└── .github/workflows/ci.yml        # GitHub Actions CI pipeline
```

---

## Roadmap

Planned next steps that extend the same vertical-slice pattern:

- **Authentication:** protect the API and scope data per user.
- **PDF invoices:** export a generated invoice as a PDF.
- **Search and filtering:** find clients and jobs quickly as the list grows.
- **Pagination:** page through large client and job lists.

---

## Author

Built by Zoe Wang: [codewithzoe.com](https://codewithzoe.com) · [Live demo](https://tradieflow.codewithzoe.com) · [github.com/zoewang66](https://github.com/zoewang66)
