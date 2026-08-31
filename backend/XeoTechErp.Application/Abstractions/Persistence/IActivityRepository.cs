using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IActivityRepository
{
    Task<IReadOnlyList<Activity>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
    void Add(Activity activity);
}
