# Architecture status

The solution now contains independent Domain, Application, Infrastructure, API and Tests projects. The core authentication, product, customer, order, inventory and dashboard flows use the new dependency-inverted application abstractions.

Some older ERP controllers and the legacy API persistence model are intentionally still present for compatibility while the remaining modules are migrated. Do not treat the migration as complete until those direct EF Core controller dependencies are removed.
