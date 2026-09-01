# Application Features

Use case code belongs here. Controllers must remain thin and must not access EF Core directly.

Recommended organization:

- Products/Commands
- Products/Queries
- Customers/Commands
- Orders/Commands
- Orders/Queries
- Inventory/Commands
- Finance/Queries

Cross-cutting behavior such as validation, logging, authorization and transactions should be applied around use cases rather than inside controllers.
