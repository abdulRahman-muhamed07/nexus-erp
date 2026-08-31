# Architecture status

The solution contains independent Domain, Application, Infrastructure, API and Tests projects. The core authentication, product, customer, order, inventory, dashboard, activity and payment flows use dependency-inverted Application abstractions with Infrastructure implementations.

The API no longer contains the old Application layer. JWT configuration is strongly typed and owned by Infrastructure, and database readiness is exposed through infrastructure health checks.

The remaining legacy API Domain model and a small number of older ERP endpoints still need migration before the presentation layer can be considered completely free of legacy compatibility code. These are intentionally kept until their routes are migrated so the existing API surface is not broken.
