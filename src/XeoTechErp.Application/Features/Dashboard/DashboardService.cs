using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Abstractions.Services;
using XeoTechErp.Application.Common.Models;

namespace XeoTechErp.Application.Services;

public sealed class DashboardService(IDashboardRepository repository) : IDashboardService
{
    public Task<DashboardMetrics> GetAsync(CancellationToken cancellationToken = default)
        => repository.GetMetricsAsync(cancellationToken);
}
