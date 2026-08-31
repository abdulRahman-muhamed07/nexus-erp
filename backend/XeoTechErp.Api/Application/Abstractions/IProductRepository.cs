using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Application.Abstractions;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> SearchAsync(string? search, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    void Remove(Product product);
}
