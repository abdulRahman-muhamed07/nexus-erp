namespace XeoTechErp.Application.Contracts.Suppliers;

public sealed record CreateSupplierRequest(string Name, string Contact, string Country, double Rating, string Email, string Phone);
