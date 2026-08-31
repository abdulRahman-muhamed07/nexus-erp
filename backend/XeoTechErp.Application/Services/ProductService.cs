using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Products;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Exceptions;

namespace XeoTechErp.Application.Services;

public sealed class ProductService(IProductRepository repository, IUnitOfWork unitOfWork) : IProductService
{
    public async Task<IReadOnlyList<ProductDto>> GetAsync(string? search, CancellationToken cancellationToken = default)
        => (await repository.SearchAsync(search, cancellationToken)).Select(ToDto).ToList();

    public async Task<ProductDto?> GetAsync(int id, CancellationToken cancellationToken = default)
        => await repository.GetByIdAsync(id, cancellationToken) is { } product ? ToDto(product) : null;

    public async Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        if (await repository.ExistsBySkuAsync(request.Sku?.Trim() ?? string.Empty, cancellationToken))
            return Result<ProductDto>.Failure("SKU_EXISTS", "SKU already exists.");

        try
        {
            var product = new Product(request.Sku, request.Name, request.Price, request.Cost, request.Stock, request.ReorderLevel, request.Category, request.SupplierId);
            repository.Add(product);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<ProductDto>.Success(ToDto(product));
        }
        catch (DomainRuleException ex)
        {
            return Result<ProductDto>.Failure("PRODUCT_INVALID", ex.Message);
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetByIdAsync(id, cancellationToken);
        if (product is null) return Result.Failure("PRODUCT_NOT_FOUND", "Product was not found.");
        repository.Remove(product);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static ProductDto ToDto(Product product) => new(product.Id, product.Sku, product.Name, product.Category, product.Price, product.Cost, product.Stock, product.ReorderLevel, product.SupplierId);
}
