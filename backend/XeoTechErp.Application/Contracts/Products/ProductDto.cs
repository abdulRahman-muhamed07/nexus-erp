namespace XeoTechErp.Application.Contracts.Products;

public sealed record ProductDto(int Id, string Sku, string Name, string Category, decimal Price, decimal Cost, int Stock, int ReorderLevel, int? SupplierId);
