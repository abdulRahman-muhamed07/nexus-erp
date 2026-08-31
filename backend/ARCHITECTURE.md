# Nexus ERP Backend Architecture

The backend follows Clean Architecture and Dependency Inversion.

## Projects
- Domain: business entities, enums and domain rules. No framework dependencies.
- Application: use cases, abstractions, DTOs and validation. Depends only on Domain.
- Infrastructure: EF Core, SQLite, repositories, authentication and external integrations.
- API: HTTP boundary, controllers, middleware and dependency-composition only.
- Tests: unit, integration and architecture tests.

## Dependency direction
API -> Application -> Domain
Infrastructure -> Application + Domain
Domain depends on nothing.

Infrastructure implementations are injected into Application abstractions from the API composition root.
