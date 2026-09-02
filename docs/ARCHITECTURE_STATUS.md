# Architecture status

The backend is organized into Domain, Application, Infrastructure, API, and Tests projects.

## Dependency direction

```text
API -> Application -> Domain
Infrastructure -> Application + Domain
```

Application exposes abstractions for persistence and external concerns. Infrastructure implements those abstractions and contains EF Core, SQL Server access, authentication implementations, and health checks. API stays focused on HTTP concerns and composition.

## Application boundaries

Feature contracts and DTOs live in Application. Domain entities own domain rules; application handlers/services coordinate use cases; infrastructure code owns persistence and technical integrations.

## Persistence

`XeoTechDbContext`, EF Core mappings, repositories, Unit of Work, and migrations belong to `XeoTechErp.Infrastructure/Persistence`.

## Security

JWT configuration is validated from application configuration, tokens validate issuer, audience, and lifetime, and authorization policies are applied at the API boundary. Secrets are not committed to the repository.

## Testing

`XeoTechErp.Tests` contains unit, mapping, integration, and architecture-focused tests that protect dependency direction and behavior.

## Design rule

Keep the dependency direction intentional. Do not add an abstraction unless it hides a real boundary or gives the application a meaningful seam for testing or replacement.
