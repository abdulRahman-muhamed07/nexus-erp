using System.ComponentModel.DataAnnotations;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Supplier
{
 public int Id { get; set; }
 [Required,MaxLength(120)] public string Name { get; set; } = null!;
 [MaxLength(120)] public string Contact { get; set; } = string.Empty;
 [MaxLength(60)] public string Country { get; set; } = string.Empty;
 public double Rating { get; set; }
 [MaxLength(120)] public string Email { get; set; } = string.Empty;
 [MaxLength(40)] public string Phone { get; set; } = string.Empty;
 public ICollection<Product> Products { get; set; } = new List<Product>();
 public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}