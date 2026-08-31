using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IPurchaseOrderRepository
{
    Task<IReadOnlyList<PurchaseOrder>> GetAsync(CancellationToken cancellationToken = default);
    Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Product?> GetProductAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> SupplierExistsAsync(int id, CancellationToken cancellationToken = default);
    void Add(PurchaseOrder purchaseOrder);
}
