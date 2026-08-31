using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Infrastructure.Persistence.Repositories;

public sealed class CustomerRepository(XeoTechDbContext db) : ICustomerRepository
{
    public async Task<IReadOnlyList<Customer>> SearchAsync(string? search, CancellationToken cancellationToken = default)
    {
        var query = db.Customers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            query = query.Where(c => c.Company.Contains(search) || c.ContactName.Contains(search));
        }

        return await query.OrderBy(c => c.Company).ToListAsync(cancellationToken);
    }

    public Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Customers.SingleOrDefaultAsync(c => c.Id == id, cancellationToken);

    public Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        db.Customers.Add(customer);
        return Task.CompletedTask;
    }
}