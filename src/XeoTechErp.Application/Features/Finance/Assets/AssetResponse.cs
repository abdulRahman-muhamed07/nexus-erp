using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Features.Finance.Assets;

public sealed record AssetResponse(int Id, string Name, string Category, DateTime PurchaseDate, decimal Cost, decimal Salvage, int UsefulLifeYears, AssetStatus Status, DateTime? DisposedOn);
