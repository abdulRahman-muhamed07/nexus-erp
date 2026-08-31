using XeoTechErp.Application.Abstractions.Persistence;

namespace XeoTechErp.Application.Services;

public interface IDashboardService
{
    Task<DashboardMetrics> GetAsync(CancellationToken cancellationToken = default);
}
