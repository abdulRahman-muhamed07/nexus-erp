using XeoTechErp.Api.Application.Abstractions;

namespace XeoTechErp.Api.Application.Services;

public interface IDashboardService
{
    Task<object> GetAsync(CancellationToken cancellationToken = default);
}

public sealed class DashboardService(IDashboardRepository repository) : IDashboardService
{
    public Task<object> GetAsync(CancellationToken cancellationToken = default) =>
        repository.GetAsync(cancellationToken);
}