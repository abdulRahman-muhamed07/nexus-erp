using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Products;

namespace XeoTechErp.Application.Services;

public interface IProductService
{
    Task<IReadOnlyList<ProductDto>> GetAsync(string? search, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
