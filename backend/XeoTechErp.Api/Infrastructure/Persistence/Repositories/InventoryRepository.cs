using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Infrastructure.Persistence.Repositories;

public sealed class InventoryRepository(XeoTechDbContext db) : IInventoryRepository
{
    public async Task<object> GetSummaryAsync(CancellationToken cancellationToken = default) =>
        await db.Products.AsNoTracking()
            .GroupBy(_ => 1)
            .Select(group => new
            {
                products = group.Count(),
                units = group.Sum(product => product.Stock),
                inventoryValue = group.Sum(product => product.Stock * product.Cost),
                lowStock = group.Count(product => product.Stock <= product.ReorderLevel)
            })
            .FirstOrDefaultAsync(cancellationToken)
        ?? new { products = 0, units = 0, inventoryValue = 0m, lowStock = 0 };

    public Task<Product?> GetProductAsync(int productId, CancellationToken cancellationToken = default) =>
        db.Products.SingleOrDefaultAsync(product => product.Id == productId, cancellationToken);

    public void AddMovement(StockMovement movement) => db.StockMovements.Add(movement);
}