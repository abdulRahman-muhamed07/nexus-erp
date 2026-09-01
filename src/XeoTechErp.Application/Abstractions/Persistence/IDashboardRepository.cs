using XeoTechErp.Application.Common.Models;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IDashboardRepository
{
    Task<DashboardMetrics> GetMetricsAsync(CancellationToken cancellationToken = default);
}
