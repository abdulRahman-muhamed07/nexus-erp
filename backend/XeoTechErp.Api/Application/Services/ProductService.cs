using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.DTOs;
using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAsync(string? search, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public sealed class ProductService(IProductRepository repository, IUnitOfWork unitOfWork) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetAsync(string? search, CancellationToken cancellationToken = default)
    {
        var products = await repository.SearchAsync(search, cancellationToken);
        return products.Select(Map).ToList();
    }

    public async Task<ProductDto?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        return product is null ? null : Map(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Sku);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Name);

        var sku = request.Sku.Trim();
        if (await repository.ExistsBySkuAsync(sku, cancellationToken))
            throw new InvalidOperationException("SKU already exists.");

        var product = new Product
        {
            Sku = sku,
            Name = request.Name.Trim(),
            Category = request.Category?.Trim() ?? string.Empty,
            Price = request.Price,
            Cost = request.Cost,
            Stock = request.Stock,
            ReorderLevel = request.ReorderLevel,
            SupplierId = request.SupplierId
        };

        await repository.AddAsync(product, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Map(product);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        if (product is null)
            return false;

        repository.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ProductDto Map(Product product) =>
        new(product.Id, product.Sku, product.Name, product.Category, product.Price, product.Cost,
            product.Stock, product.ReorderLevel, product.SupplierId);
}
