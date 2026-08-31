using System.ComponentModel.DataAnnotations;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class OrderItem
{
 public int Id { get; set; }
 public int OrderId { get; set; }
 public Order Order { get; set; } = null!;
 public int ProductId { get; set; }
 [MaxLength(120)] public string Name { get; set; } = string.Empty;
 public int Qty { get; set; }
 public decimal Price { get; set; }
}