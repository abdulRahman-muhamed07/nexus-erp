namespace XeoTechErp.Application.Contracts.Suppliers;

public sealed record SupplierDetailsResponse(int Id, string Name, string Contact, string Country, double Rating, string Email, string Phone, int ProductCount, int PurchaseOrderCount);
