using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IAppConfigRepository
{
    Task<AppConfig?> GetAsync(CancellationToken cancellationToken = default);
    void Add(AppConfig config);
}
