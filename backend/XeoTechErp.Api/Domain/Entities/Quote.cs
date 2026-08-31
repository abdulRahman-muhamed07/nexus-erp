using XeoTechErp.Api.Domain.Enums;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Quote
{
 public int Id { get; set; }
 public int CustomerId { get; set; }
 public Customer Customer { get; set; } = null!;
 public QuoteStatus Status { get; set; } = QuoteStatus.Draft;
 public DateTime Date { get; set; } = DateTime.UtcNow;
 public decimal Subtotal { get; set; }
 public decimal Tax { get; set; }
 public decimal Shipping { get; set; }
 public decimal Total { get; set; }
 public decimal DiscountPct { get; set; }
 public ICollection<QuoteItem> Items { get; set; } = new List<QuoteItem>();
}