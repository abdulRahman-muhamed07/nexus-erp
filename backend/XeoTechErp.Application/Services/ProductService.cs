using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Products;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAsync(string? search, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public sealed class ProductService(IProductRepository repository, IUnitOfWork unitOfWork) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetAsync(string? search, CancellationToken cancellationToken = default)
        => (await repository.SearchAsync(search, cancellationToken)).Select(ToDto).ToList();

    public async Task<ProductDto?> GetAsync(int id, CancellationToken cancellationToken = default)
        => await repository.GetByIdAsync(id, cancellationToken) is { } product ? ToDto(product) : null;

    public async Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Sku) || string.IsNullOrWhiteSpace(request.Name))
            return Result<ProductDto>.Failure("PRODUCT_INVALID", "SKU and product name are required.");
        if (request.Price < 0 || request.Cost < 0 || request.Stock < 0 || request.ReorderLevel < 0)
            return Result<ProductDto>.Failure("PRODUCT_INVALID", "Price, cost, stock and reorder level cannot be negative.");
        var sku = request.Sku.Trim();
        if (await repository.ExistsBySkuAsync(sku, cancellationToken))
            return Result<ProductDto>.Failure("SKU_EXISTS", "SKU already exists.");

        var product = new Product { Sku = sku, Name = request.Name.Trim(), Category = request.Category?.Trim() ?? string.Empty, Price = request.Price, Cost = request.Cost, Stock = request.Stock, ReorderLevel = request.ReorderLevel, SupplierId = request.SupplierId };
        repository.Add(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<ProductDto>.Success(ToDto(product));
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        if (product is null) return Result.Failure("PRODUCT_NOT_FOUND", "Product was not found.");
        repository.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static ProductDto ToDto(Product p) => new(p.Id, p.Sku, p.Name, p.Category, p.Price, p.Cost, p.Stock, p.ReorderLevel, p.SupplierId);
}
