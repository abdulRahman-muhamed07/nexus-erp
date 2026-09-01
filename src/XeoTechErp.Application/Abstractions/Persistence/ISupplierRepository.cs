using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface ISupplierRepository
{
    Task<IReadOnlyList<Supplier>> SearchAsync(string? search, CancellationToken cancellationToken = default);
    Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> HasReferencesAsync(int id, CancellationToken cancellationToken = default);
    void Add(Supplier supplier);
    void Remove(Supplier supplier);
}
