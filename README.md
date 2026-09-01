# Nexus ERP — Business Suite

Nexus ERP is a full-stack business management system with a vanilla HTML/CSS/JavaScript frontend and an ASP.NET Core backend.

## Architecture

The backend follows Clean Architecture:

```text
src/
├── XeoTechErp.Api            # HTTP layer, controllers, middleware
├── XeoTechErp.Application    # use cases, contracts, abstractions
├── XeoTechErp.Domain         # entities, enums, domain rules
└── XeoTechErp.Infrastructure # EF Core, repositories, authentication, health checks
```

The frontend is kept separately under `front/`:

```text
front/
├── html/
├── css/
└── js/
```

## Backend stack

- .NET 10 / ASP.NET Core Web API
- C#
- Entity Framework Core
- SQL Server/relational persistence through the Infrastructure layer
- JWT authentication with refresh tokens
- AutoMapper
- xUnit tests
- GitHub Actions CI

## Backend features

- Authentication and refresh-token flow
- Customers and suppliers
- Products and inventory
- Sales orders and quotes
- Payments, invoices, returns
- Procurement and purchase orders
- HR and employees
- Finance, budgets, expenses, assets and depreciation
- Reports and dashboard metrics
- Notifications and audit logging
- Health checks and centralized exception handling

## Frontend

The frontend is a standalone vanilla JavaScript client. Its files are separated into HTML, CSS and JavaScript under `front/`.

## Run locally

### Backend

```bash
dotnet restore XeoTechErp.sln
dotnet build XeoTechErp.sln
dotnet test XeoTechErp.sln
```

Then run:

```bash
dotnet run --project src/XeoTechErp.Api/XeoTechErp.Api.csproj
```

### Frontend

From the repository root:

```bash
python -m http.server 8080 -d front
```

Then open the frontend entry page from the `front/html` directory using the server URL.

## Repository structure

- `src/` — .NET backend
- `front/` — frontend assets
- `tests/` — automated tests
- `docs/` — architecture and configuration documentation
- `XeoTechErp.sln` — backend solution

## Quality gates

Every backend change is checked by GitHub Actions with restore, Release build and automated tests.
