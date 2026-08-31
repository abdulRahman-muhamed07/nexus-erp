using System.ComponentModel.DataAnnotations;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Return
{
 public int Id { get; set; }
 public int OrderId { get; set; }
 public Order Order { get; set; } = null!;
 public decimal Amount { get; set; }
 [MaxLength(200)] public string Reason { get; set; } = string.Empty;
 public DateTime Date { get; set; } = DateTime.UtcNow;
}