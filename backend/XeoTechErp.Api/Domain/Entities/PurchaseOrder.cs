using System.ComponentModel.DataAnnotations;
using XeoTechErp.Api.Domain.Enums;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class PurchaseOrder
{
 public int Id { get; set; }
 public int SupplierId { get; set; }
 public Supplier Supplier { get; set; } = null!;
 public int ProductId { get; set; }
 [MaxLength(120)] public string ProductName { get; set; } = string.Empty;
 public int Qty { get; set; }
 public decimal Cost { get; set; }
 public PoStatus Status { get; set; } = PoStatus.Pending;
 public DateTime Eta { get; set; }
 public DateTime Created { get; set; } = DateTime.UtcNow;
}