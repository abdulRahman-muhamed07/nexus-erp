# Nexus ERP Backend

The backend is being maintained as a Clean Architecture solution with explicit dependency inversion.

## Structure

```text
XeoTechErp
├── XeoTechErp.Domain
├── XeoTechErp.Application
├── XeoTechErp.Infrastructure
├── XeoTechErp.Api
└── XeoTechErp.Tests
```

### Domain
Pure business model. No EF Core, ASP.NET, configuration or infrastructure dependencies.

### Application
Use cases, DTOs, validation/contracts, repository abstractions and business orchestration.

### Infrastructure
EF Core/SQLite persistence, repository implementations, JWT and password verification.

### API
HTTP boundary and composition root. Controllers should contain transport concerns only.

### Tests
Domain and architectural dependency tests are kept separate from the API boundary.

## Dependency rule

```text
API ───────────────► Application ─────► Domain
Infrastructure ───► Application ─────► Domain
```

Infrastructure implements the interfaces defined by Application. This keeps business logic independent of the database and frameworks.
