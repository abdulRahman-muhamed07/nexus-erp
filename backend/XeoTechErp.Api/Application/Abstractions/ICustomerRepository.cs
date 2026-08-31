using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Application.Abstractions;

public interface ICustomerRepository
{
    Task<IReadOnlyList<Customer>> SearchAsync(string? search, CancellationToken cancellationToken = default);
    Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(Customer customer, CancellationToken cancellationToken = default);
}