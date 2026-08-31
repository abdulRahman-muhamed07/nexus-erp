using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class PurchaseOrderRepository(XeoTechDbContext db) : IPurchaseOrderRepository
{
    public async Task<IReadOnlyList<PurchaseOrder>> GetAsync(CancellationToken cancellationToken = default)
        => await db.PurchaseOrders.AsNoTracking().OrderByDescending(x => x.Created).ToListAsync(cancellationToken);

    public Task<PurchaseOrder?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => db.PurchaseOrders.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Product?> GetProductAsync(int id, CancellationToken cancellationToken = default)
        => db.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> SupplierExistsAsync(int id, CancellationToken cancellationToken = default)
        => db.Suppliers.AnyAsync(x => x.Id == id, cancellationToken);

    public void Add(PurchaseOrder purchaseOrder) => db.PurchaseOrders.Add(purchaseOrder);
}
