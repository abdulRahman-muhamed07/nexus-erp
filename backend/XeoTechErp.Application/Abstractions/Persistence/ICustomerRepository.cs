using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface ICustomerRepository : IRepository<Customer>
{
    Task<IReadOnlyList<Customer>> SearchAsync(string? search, CancellationToken cancellationToken = default);
}
