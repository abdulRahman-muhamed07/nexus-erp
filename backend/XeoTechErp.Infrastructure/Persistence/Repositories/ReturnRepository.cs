using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class ReturnRepository(XeoTechDbContext db) : IReturnRepository
{
    public async Task<IReadOnlyList<Return>> GetAsync(CancellationToken cancellationToken = default)
        => await db.Returns.AsNoTracking().OrderByDescending(x => x.Date).ToListAsync(cancellationToken);

    public Task<Order?> GetDeliveredOrderWithItemsAsync(int orderId, CancellationToken cancellationToken = default)
        => db.Orders.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == orderId && x.Status == OrderStatus.Delivered, cancellationToken);

    public void Add(Return @return) => db.Returns.Add(@return);
}
