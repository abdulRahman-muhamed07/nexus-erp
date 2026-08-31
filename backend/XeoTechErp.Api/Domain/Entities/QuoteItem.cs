using System.ComponentModel.DataAnnotations;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class QuoteItem
{
 public int Id { get; set; }
 public int QuoteId { get; set; }
 public Quote Quote { get; set; } = null!;
 public int ProductId { get; set; }
 [MaxLength(120)] public string Name { get; set; } = string.Empty;
 public int Qty { get; set; }
 public decimal Price { get; set; }
}