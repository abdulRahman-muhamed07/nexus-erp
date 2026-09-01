using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Domain.Entities;

public sealed class Asset
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
    public decimal Cost { get; set; }
    public decimal Salvage { get; set; }
    public int UsefulLifeYears { get; set; } = 5;
    public AssetStatus Status { get; set; } = AssetStatus.InService;
    public DateTime? DisposedOn { get; set; }
}
