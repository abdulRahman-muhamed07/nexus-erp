using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class ActivityRepository(XeoTechDbContext db) : IActivityRepository
{
    public async Task<IReadOnlyList<Activity>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
        => await db.Activities.AsNoTracking().OrderByDescending(x => x.Time).Take(take).ToListAsync(cancellationToken);

    public void Add(Activity activity) => db.Activities.Add(activity);
}
