using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.Finance;

public sealed record AssetResponse(int Id, string Name, string Category, DateTime PurchaseDate, decimal Cost, decimal Salvage, int UsefulLifeYears, AssetStatus Status, DateTime? DisposedOn);
