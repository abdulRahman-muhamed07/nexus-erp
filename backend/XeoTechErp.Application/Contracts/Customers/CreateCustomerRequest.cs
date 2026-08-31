using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.Customers;

public sealed record CreateCustomerRequest(string Company, string ContactName, string Email, string Phone, string Country, CustomerTier Tier, string PaymentTerms, decimal CreditLimit);
