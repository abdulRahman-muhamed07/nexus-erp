using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Customers;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Services;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAsync(string? search, CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<CustomerDto>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}

public sealed class CustomerService(ICustomerRepository repository, IUnitOfWork unitOfWork) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerDto>> GetAsync(string? search, CancellationToken cancellationToken = default)
        => (await repository.SearchAsync(search, cancellationToken)).Select(ToDto).ToList();

    public async Task<CustomerDto?> GetAsync(int id, CancellationToken cancellationToken = default)
        => await repository.GetByIdAsync(id, cancellationToken) is { } customer ? ToDto(customer) : null;

    public async Task<Result<CustomerDto>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Company))
            return Result<CustomerDto>.Failure("CUSTOMER_INVALID", "Company is required.");
        if (request.CreditLimit < 0)
            return Result<CustomerDto>.Failure("CUSTOMER_INVALID", "Credit limit cannot be negative.");

        var customer = new Customer { Company = request.Company.Trim(), ContactName = request.ContactName?.Trim() ?? string.Empty, Email = request.Email?.Trim() ?? string.Empty, Phone = request.Phone?.Trim() ?? string.Empty, Country = request.Country?.Trim() ?? string.Empty, Tier = request.Tier, PaymentTerms = string.IsNullOrWhiteSpace(request.PaymentTerms) ? "Net 30" : request.PaymentTerms.Trim(), CreditLimit = request.CreditLimit };
        repository.Add(customer);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CustomerDto>.Success(ToDto(customer));
    }

    private static CustomerDto ToDto(Customer c) => new(c.Id, c.Company, c.ContactName, c.Email, c.Phone, c.Country, c.Tier, c.PaymentTerms, c.CreditLimit, c.OnHold);
}
