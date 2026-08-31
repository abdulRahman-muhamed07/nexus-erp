using System.ComponentModel.DataAnnotations;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class StockMovement
{
 public int Id { get; set; }
 public int? ProductId { get; set; }
 [MaxLength(120)] public string ProductName { get; set; } = string.Empty;
 public int Delta { get; set; }
 [MaxLength(40)] public string Reason { get; set; } = "Adjustment";
 [MaxLength(40)] public string? Reference { get; set; }
 [MaxLength(120)] public string By { get; set; } = string.Empty;
 public DateTime Time { get; set; } = DateTime.UtcNow;
}