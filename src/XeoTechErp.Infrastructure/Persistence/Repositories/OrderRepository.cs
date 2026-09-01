using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(XeoTechDbContext db) : EfRepository<Order>(db), IOrderRepository
{
    public Task<Order?> GetWithItemsAsync(int id, CancellationToken cancellationToken = default)
        => db.Orders.AsNoTracking().Include(x => x.Items).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
        => await db.Orders.AsNoTracking().OrderByDescending(x => x.OrderDate).ToListAsync(cancellationToken);
}
