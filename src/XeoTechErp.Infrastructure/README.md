# Infrastructure

Infrastructure owns EF Core, SQLite, repositories and authentication implementations.
Application code depends only on abstractions; concrete infrastructure types are registered through `AddInfrastructure`.

Database connection configuration belongs here and is supplied by the API composition root.
