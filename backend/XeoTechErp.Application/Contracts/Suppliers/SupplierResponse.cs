namespace XeoTechErp.Application.Contracts.Suppliers;

public sealed record SupplierResponse(int Id, string Name, string Contact, string Country, double Rating, string Email, string Phone);
