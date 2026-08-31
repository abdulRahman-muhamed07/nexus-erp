namespace XeoTechErp.Application.Contracts.Products;

public sealed record CreateProductRequest(string Sku, string Name, string Category, decimal Price, decimal Cost, int Stock, int ReorderLevel, int? SupplierId);
