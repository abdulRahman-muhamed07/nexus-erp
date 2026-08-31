using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class DashboardRepository(XeoTechDbContext db) : IDashboardRepository
{
    public async Task<DashboardMetrics> GetMetricsAsync(CancellationToken cancellationToken = default)
    {
        var revenue = await db.Orders.Where(x => x.Status != OrderStatus.Cancelled).SumAsync(x => (decimal?)x.Total, cancellationToken) ?? 0m;
        var orders = await db.Orders.CountAsync(cancellationToken);
        var customers = await db.Customers.CountAsync(cancellationToken);
        var products = await db.Products.CountAsync(cancellationToken);
        var lowStock = await db.Products.CountAsync(x => x.Stock <= x.ReorderLevel, cancellationToken);
        return new DashboardMetrics(revenue, orders, customers, products, lowStock);
    }
}
