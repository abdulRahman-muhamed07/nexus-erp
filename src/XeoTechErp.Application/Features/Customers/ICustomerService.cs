using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Customers;

namespace XeoTechErp.Application.Abstractions.Services;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAsync(string? search, CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<CustomerDto>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}
