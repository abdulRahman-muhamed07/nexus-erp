using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(XeoTechDbContext db) : EfRepository<Customer>(db), ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> SearchAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = db.Customers.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(x => x.Company.Contains(search) || x.ContactName.Contains(search));
        return await query.OrderBy(x => x.Company).ToListAsync(cancellationToken);
    }
}
