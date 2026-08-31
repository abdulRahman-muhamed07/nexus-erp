namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IDashboardRepository
{
    Task<DashboardMetrics> GetMetricsAsync(CancellationToken cancellationToken = default);
}

public sealed record DashboardMetrics(decimal Revenue, int Orders, int Customers, int Products, int LowStock);
