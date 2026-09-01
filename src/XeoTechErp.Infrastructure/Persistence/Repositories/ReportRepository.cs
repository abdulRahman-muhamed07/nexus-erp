using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class ReportRepository(XeoTechDbContext db) : IReportRepository
{
    public async Task<(int TotalOrders, decimal Revenue, decimal AverageOrderValue)> GetSalesSummaryAsync(CancellationToken cancellationToken = default)
    {
        var orders = db.Orders.AsNoTracking().Where(x => x.Status != OrderStatus.Cancelled);
        var totalOrders = await orders.CountAsync(cancellationToken);
        var revenue = await orders.SumAsync(x => x.Total, cancellationToken);
        var average = totalOrders == 0 ? 0m : await orders.AverageAsync(x => x.Total, cancellationToken);
        return (totalOrders, revenue, average);
    }
}
