using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> SearchAsync(string? search, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task<Dictionary<int, Product>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
