using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.Domain.Enums;

namespace XeoTechErp.Api.Infrastructure.Persistence.Repositories;

public sealed class DashboardRepository(XeoTechDbContext db) : IDashboardRepository
{
    public async Task<object> GetAsync(CancellationToken cancellationToken = default) => new
    {
        revenue = await db.Orders.Where(order => order.Status != OrderStatus.Cancelled).SumAsync(order => (decimal?)order.Total, cancellationToken) ?? 0m,
        orders = await db.Orders.CountAsync(cancellationToken),
        customers = await db.Customers.CountAsync(cancellationToken),
        products = await db.Products.CountAsync(cancellationToken),
        lowStock = await db.Products.CountAsync(product => product.Stock <= product.ReorderLevel, cancellationToken)
    };
}