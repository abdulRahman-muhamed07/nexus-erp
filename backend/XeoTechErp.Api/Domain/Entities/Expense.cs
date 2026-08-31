using System.ComponentModel.DataAnnotations;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Expense
{
 public int Id { get; set; }
 [Required,MaxLength(60)] public string Category { get; set; } = string.Empty;
 public decimal Amount { get; set; }
 public DateTime Date { get; set; } = DateTime.UtcNow;
 [MaxLength(500)] public string Description { get; set; } = string.Empty;
}