using System.ComponentModel.DataAnnotations;
using XeoTechErp.Api.Domain.Enums;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Invoice
{
 public int Id { get; set; }
 public int? OrderId { get; set; }
 public Order? Order { get; set; }
 public int? CustomerId { get; set; }
 public Customer? Customer { get; set; }
 [MaxLength(120)] public string CustomerName { get; set; } = string.Empty;
 public decimal Amount { get; set; }
 public DateTime Issued { get; set; } = DateTime.UtcNow;
 public DateTime Due { get; set; }
 public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;
 public DateTime? PaidOn { get; set; }
}