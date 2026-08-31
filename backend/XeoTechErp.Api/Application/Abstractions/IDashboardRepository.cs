namespace XeoTechErp.Api.Application.Abstractions;

public interface IDashboardRepository
{
    Task<object> GetAsync(CancellationToken cancellationToken = default);
}