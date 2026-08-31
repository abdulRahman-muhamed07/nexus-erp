using System.ComponentModel.DataAnnotations;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class AuditLogEntry
{
 public int Id { get; set; }
 public DateTime Time { get; set; } = DateTime.UtcNow;
 [MaxLength(120)] public string User { get; set; } = string.Empty;
 [MaxLength(30)] public string Role { get; set; } = string.Empty;
 [MaxLength(60)] public string Icon { get; set; } = string.Empty;
 [MaxLength(120)] public string Action { get; set; } = string.Empty;
 [MaxLength(60)] public string Module { get; set; } = string.Empty;
 [MaxLength(160)] public string Target { get; set; } = string.Empty;
 [MaxLength(500)] public string Detail { get; set; } = string.Empty;
}