using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class InventoryRepository(XeoTechDbContext db) : IInventoryRepository
{
    public async Task<(int Products, int Units, decimal InventoryValue, int LowStock)> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var result = await db.Products.AsNoTracking().GroupBy(_ => 1).Select(g => new { Products = g.Count(), Units = g.Sum(x => x.Stock), InventoryValue = g.Sum(x => x.Stock * x.Cost), LowStock = g.Count(x => x.Stock <= x.ReorderLevel) }).FirstOrDefaultAsync(cancellationToken);
        return result is null ? (0, 0, 0m, 0) : (result.Products, result.Units, result.InventoryValue, result.LowStock);
    }
    public void AddMovement(StockMovement movement) => db.StockMovements.Add(movement);
}
