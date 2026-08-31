using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class SupplierRepository(XeoTechDbContext db) : ISupplierRepository
{
    public Task<IReadOnlyList<Supplier>> SearchAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = db.Suppliers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Name.Contains(search) || x.Email.Contains(search));
        return query.OrderBy(x => x.Name).ToListAsync(cancellationToken).ContinueWith(t => (IReadOnlyList<Supplier>)t.Result, cancellationToken);
    }

    public Task<Supplier?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Suppliers.AsNoTracking().Include(x => x.Products).Include(x => x.PurchaseOrders).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<bool> HasReferencesAsync(int id, CancellationToken cancellationToken = default) =>
        await db.Products.AnyAsync(x => x.SupplierId == id, cancellationToken) ||
        await db.PurchaseOrders.AnyAsync(x => x.SupplierId == id, cancellationToken);

    public void Add(Supplier supplier) => db.Suppliers.Add(supplier);
    public void Remove(Supplier supplier) => db.Suppliers.Remove(supplier);
}
