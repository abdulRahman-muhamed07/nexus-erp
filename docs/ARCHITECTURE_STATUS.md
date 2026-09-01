# Architecture status

The backend is organized as independent Domain, Application, Infrastructure, API and Tests projects.

## Dependency direction

`API -> Application -> Domain`

`Infrastructure -> Application + Domain`

The Application layer exposes abstractions for persistence and external concerns. Infrastructure contains EF Core, SQLite, repositories, authentication implementations and health checks. The API is limited to HTTP concerns, middleware, configuration composition and controllers.

## Application boundaries

Application contracts use explicit request and response DTOs. AutoMapper owns entity-to-contract mapping. Business rules are enforced by Domain entities, while Application handlers/services orchestrate use cases and transactions.

## Persistence

`XeoTechDbContext`, EF Core mappings, repositories, Unit of Work and migrations belong to `XeoTechErp.Infrastructure/Persistence`.

## Security

JWT options are validated from secure configuration, tokens validate issuer, audience and lifetime, login is rate limited, and authorization policies are defined for administrator/manager operations. Secrets are not committed to the repository.

## Testing

The solution keeps unit, mapping and architecture-focused tests in `XeoTechErp.Tests`.

## Design rule

The API must never depend directly on EF Core or Domain persistence models. New features should be added as application use cases with dedicated contracts, abstractions and infrastructure implementations.
