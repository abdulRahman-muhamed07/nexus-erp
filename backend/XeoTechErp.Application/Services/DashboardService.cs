using XeoTechErp.Application.Abstractions.Persistence;

namespace XeoTechErp.Application.Services;

public interface IDashboardService
{
    Task<DashboardMetrics> GetAsync(CancellationToken cancellationToken = default);
}

public sealed class DashboardService(IDashboardRepository repository) : IDashboardService
{
    public Task<DashboardMetrics> GetAsync(CancellationToken cancellationToken = default) => repository.GetMetricsAsync(cancellationToken);
}
