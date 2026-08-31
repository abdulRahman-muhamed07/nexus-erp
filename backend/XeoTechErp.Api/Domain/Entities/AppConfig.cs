namespace XeoTechErp.Api.Domain.Entities;
public sealed class AppConfig
{
 public int Id { get; set; }
 public decimal TaxRate { get; set; } = 8m;
 public decimal ShippingFee { get; set; } = 25m;
 public decimal FreeShipOver { get; set; } = 1000m;
}