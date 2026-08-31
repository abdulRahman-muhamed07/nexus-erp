using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.Customers;

public sealed record CustomerDto(int Id, string Company, string ContactName, string Email, string Phone, string Country, CustomerTier Tier, string PaymentTerms, decimal CreditLimit, bool OnHold);
public sealed record CreateCustomerRequest(string Company, string ContactName, string Email, string Phone, string Country, CustomerTier Tier, string PaymentTerms, decimal CreditLimit);
