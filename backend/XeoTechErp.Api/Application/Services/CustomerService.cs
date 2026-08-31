using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.Domain.Entities;
using XeoTechErp.Api.DTOs;

namespace XeoTechErp.Api.Application.Services;

public interface ICustomerService
{
    Task<IReadOnlyList<CustomerDto>> GetAsync(string? search, CancellationToken cancellationToken = default);
    Task<CustomerDto?> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
}

public sealed class CustomerService(ICustomerRepository repository, IUnitOfWork unitOfWork) : ICustomerService
{
    public async Task<IReadOnlyList<CustomerDto>> GetAsync(string? search, CancellationToken cancellationToken = default)
    {
        var customers = await repository.SearchAsync(search, cancellationToken);
        return customers.Select(Map).ToList();
    }

    public async Task<CustomerDto?> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var customer = await repository.GetByIdAsync(id, cancellationToken);
        return customer is null ? null : Map(customer);
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.Company);

        var customer = new Customer
        {
            Company = request.Company.Trim(),
            ContactName = request.ContactName?.Trim() ?? string.Empty,
            Email = request.Email?.Trim() ?? string.Empty,
            Phone = request.Phone?.Trim() ?? string.Empty,
            Country = request.Country?.Trim() ?? string.Empty,
            Tier = request.Tier,
            PaymentTerms = request.PaymentTerms?.Trim() ?? "Net 30",
            CreditLimit = request.CreditLimit
        };

        await repository.AddAsync(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Map(customer);
    }

    private static CustomerDto Map(Customer customer) =>
        new(customer.Id, customer.Company, customer.ContactName, customer.Email, customer.Phone,
            customer.Country, customer.Tier, customer.PaymentTerms, customer.CreditLimit, customer.OnHold);
}
