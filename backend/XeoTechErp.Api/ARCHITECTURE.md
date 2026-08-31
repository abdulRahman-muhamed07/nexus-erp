# Nexus ERP Backend Architecture

## Layers

### Domain
`Domain/Entities` contains business entities only. Each entity has its own file. `Domain/Enums` contains domain enums. The Domain layer does not know about EF Core, HTTP, configuration, or infrastructure.

### Application
`Application/Services` contains use cases and business orchestration. `Application/Abstractions` contains contracts for persistence and external concerns. Application services depend on interfaces, not concrete EF Core implementations.

### Infrastructure
`Infrastructure/Persistence` contains `XeoTechDbContext`, repository implementations, and the unit of work. `Infrastructure/Security` contains the JWT implementation. Database seeding also belongs here.

### Presentation
`Controllers`, `Middleware`, and `Program.cs` form the API/presentation boundary. `Program.cs` is the composition root where the abstractions are connected to infrastructure implementations.

## Dependency direction

`Presentation -> Application -> Domain`

`Infrastructure -> Application + Domain`

The Application layer never references Infrastructure. This keeps the dependency direction aligned with Dependency Inversion and makes application services easier to test.

## Example request flow

`ProductsController -> IProductService -> IProductRepository -> ProductRepository -> XeoTechDbContext -> SQLite`

## Persistence location

EF Core code lives under `Infrastructure/Persistence`. The database connection remains configuration-driven through `appsettings*.json` and is registered only from the composition root/infrastructure registration.
