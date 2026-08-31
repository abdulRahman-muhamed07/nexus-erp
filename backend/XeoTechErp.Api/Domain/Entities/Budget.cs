using System.ComponentModel.DataAnnotations;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Budget
{
 public int Id { get; set; }
 [Required,MaxLength(60)] public string Category { get; set; } = null!;
 public decimal MonthlyAmount { get; set; }
}