using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Suppliers;

namespace XeoTechErp.Application.Services;

public interface ISupplierService
{
    Task<IReadOnlyList<SupplierResponse>> GetAsync(string? search, CancellationToken cancellationToken = default);
    Task<SupplierDetailsResponse?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<SupplierResponse>> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result<SupplierResponse>> UpdateAsync(int id, UpdateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
