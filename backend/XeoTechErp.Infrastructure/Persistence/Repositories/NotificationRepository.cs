using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class NotificationRepository(XeoTechDbContext db) : INotificationRepository
{
    public async Task<IReadOnlyList<Notification>> GetAsync(bool unreadOnly, int take, CancellationToken cancellationToken = default)
    {
        var query = db.Notifications.AsNoTracking();
        if (unreadOnly) query = query.Where(x => !x.IsRead);
        return await query.OrderByDescending(x => x.Time).Take(take).ToListAsync(cancellationToken);
    }

    public Task<Notification?> GetByIdAsync(int id, CancellationToken cancellationToken = default) => db.Notifications.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    public async Task MarkAllReadAsync(CancellationToken cancellationToken = default) => await db.Notifications.Where(x => !x.IsRead).ExecuteUpdateAsync(x => x.SetProperty(n => n.IsRead, true), cancellationToken);
}
