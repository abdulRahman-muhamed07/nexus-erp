# Application Layer

The Application layer contains use cases, service contracts, DTOs, and persistence abstractions.

Rules:
- No Entity Framework Core references.
- No `DbContext` references.
- No configuration access such as `IConfiguration` for infrastructure concerns.
- Depend on abstractions defined in `Application/Abstractions`.

Flow:

`Controller -> Application Service -> Application Abstraction -> Infrastructure Implementation -> EF Core -> SQLite`
