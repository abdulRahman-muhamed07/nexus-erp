namespace XeoTechErp.Application.Abstractions.Persistence;

public sealed record DashboardMetrics(decimal Revenue, int Orders, int Customers, int Products, int LowStock);
