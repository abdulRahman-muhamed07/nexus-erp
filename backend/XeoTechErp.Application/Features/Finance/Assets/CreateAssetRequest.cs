namespace XeoTechErp.Application.Features.Finance.Assets;

public sealed record CreateAssetRequest(string Name, string Category, DateTime PurchaseDate, decimal Cost, decimal Salvage, int UsefulLifeYears);
