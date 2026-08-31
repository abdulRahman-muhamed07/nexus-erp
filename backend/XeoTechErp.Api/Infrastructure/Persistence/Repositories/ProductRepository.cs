using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Infrastructure.Persistence.Repositories;

public sealed class ProductRepository(XeoTechDbContext db) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> SearchAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = db.Products.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(p => p.Name.Contains(search) || p.Sku.Contains(search));
        }

        return await query.OrderBy(p => p.Name).ToListAsync(cancellationToken);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Products.SingleOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default) =>
        db.Products.AnyAsync(p => p.Sku == sku, cancellationToken);

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        db.Products.Add(product);
        return Task.CompletedTask;
    }

    public void Remove(Product product) => db.Products.Remove(product);
}