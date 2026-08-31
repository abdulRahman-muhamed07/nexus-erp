using XeoTechErp.Api.Domain.Enums;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Payment
{
 public int Id { get; set; }
 public int OrderId { get; set; }
 public Order Order { get; set; } = null!;
 public decimal Amount { get; set; }
 public PaymentMethod Method { get; set; } = PaymentMethod.Other;
 public DateTime Date { get; set; } = DateTime.UtcNow;
}