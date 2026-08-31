using System.ComponentModel.DataAnnotations;
using XeoTechErp.Api.Domain.Enums;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Asset
{
 public int Id { get; set; }
 [Required,MaxLength(160)] public string Name { get; set; } = null!;
 [MaxLength(60)] public string Category { get; set; } = string.Empty;
 public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
 public decimal Cost { get; set; }
 public decimal Salvage { get; set; }
 public int UsefulLifeYears { get; set; } = 5;
 public AssetStatus Status { get; set; } = AssetStatus.InService;
 public DateTime? DisposedOn { get; set; }
}