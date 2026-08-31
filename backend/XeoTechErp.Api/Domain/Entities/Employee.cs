using System.ComponentModel.DataAnnotations;
using XeoTechErp.Api.Domain.Enums;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Employee
{
 public int Id { get; set; }
 [Required,MaxLength(120)] public string Name { get; set; } = null!;
 [MaxLength(120)] public string JobTitle { get; set; } = string.Empty;
 [MaxLength(60)] public string Department { get; set; } = string.Empty;
 [MaxLength(120)] public string Email { get; set; } = string.Empty;
 public decimal Salary { get; set; }
 public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;
 public DateTime HireDate { get; set; } = DateTime.UtcNow;
}