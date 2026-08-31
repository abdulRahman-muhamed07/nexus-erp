using XeoTechErp.Api.Domain.Enums;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Order
{
 public int Id { get; set; }
 public int CustomerId { get; set; }
 public Customer Customer { get; set; } = null!;
 public OrderStatus Status { get; set; } = OrderStatus.Pending;
 public DateTime OrderDate { get; set; } = DateTime.UtcNow;
 public decimal Subtotal { get; set; }
 public decimal Tax { get; set; }
 public decimal Shipping { get; set; }
 public decimal Total { get; set; }
 public decimal DiscountPct { get; set; }
 public decimal Discount { get; set; }
 public int? QuoteId { get; set; }
 public Quote? Quote { get; set; }
 public int? InvoiceId { get; set; }
 public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
 public ICollection<Payment> Payments { get; set; } = new List<Payment>();
 public ICollection<Return> Returns { get; set; } = new List<Return>();
}