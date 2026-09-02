# Nexus ERP Backend Architecture

The backend follows Clean Architecture with dependency inversion between application code and infrastructure details.

## Projects

- `XeoTechErp.Domain` — entities, enums, and domain rules. It has no dependency on the outer application or infrastructure layers.
- `XeoTechErp.Application` — feature use cases, contracts, validation, and abstractions for persistence or external concerns. It depends only on Domain.
- `XeoTechErp.Infrastructure` — SQL Server/EF Core persistence, repositories, authentication implementations, external integrations, and health checks.
- `XeoTechErp.Api` — HTTP concerns, controllers, middleware, configuration, and dependency composition.
- `XeoTechErp.Tests` — unit, mapping, integration, and architecture tests.

## Dependency direction

```text
API -> Application -> Domain
Infrastructure -> Application + Domain
```

The API composition root wires infrastructure implementations to application abstractions. The Application project does not reference Infrastructure or API.

## Persistence

SQL Server is the relational provider used by the backend. EF Core `DbContext`, entity configurations, migrations, repositories, and Unit of Work implementations live under Infrastructure persistence code.

## Design rules

Domain code stays framework-independent. Application code describes the system's use cases without knowing how persistence or external services are implemented. Infrastructure owns technical details, while controllers stay focused on HTTP concerns.
