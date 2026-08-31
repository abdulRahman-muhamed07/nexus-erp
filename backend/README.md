# Nexus ERP Backend

ASP.NET Core Web API for Nexus ERP.

## Structure

```text
backend/
├── XeoTechErp.Api/
│   ├── Controllers/        # HTTP endpoints grouped by business area
│   ├── DTOs/               # Request/response contracts
│   ├── Models/             # Persistence/domain models
│   ├── Services/           # Business/application services
│   ├── Infrastructure/     # Persistence, security and cross-cutting integrations
│   ├── Middleware/         # Global HTTP pipeline concerns
│   ├── Data/               # EF Core DbContext and migrations
│   ├── Configuration/      # Typed configuration and options
│   ├── Extensions/         # Dependency injection / application extensions
│   ├── appsettings*.json   # Environment configuration (secrets supplied externally)
│   └── Program.cs          # Composition root
└── XeoTechErp.Api.Tests/    # Automated backend tests
```

## Architecture rule

Controllers stay thin. Business rules belong in services/domain logic. Database access stays behind the data/infrastructure boundary. DTOs are used at API boundaries.

## Local development

```bash
dotnet restore
dotnet build
 dotnet test
 dotnet run --project XeoTechErp.Api
```

Set `Jwt:Key` through environment variables or user secrets; never commit production secrets.
