using XeoTechErp.Application.Common.Models;

namespace XeoTechErp.Application.Abstractions.Services;

public interface IDashboardService
{
    Task<DashboardMetrics> GetAsync(CancellationToken cancellationToken = default);
}
