using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(XeoTechDbContext db) : EfRepository<Product>(db), IProductRepository
{
    public Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default) => db.Products.AnyAsync(x => x.Sku == sku, cancellationToken);
    public async Task<IReadOnlyList<Product>> SearchAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = db.Products.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Name.Contains(search) || x.Sku.Contains(search));
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }
    public async Task<Dictionary<int, Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        => await db.Products.Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id, cancellationToken);
}
