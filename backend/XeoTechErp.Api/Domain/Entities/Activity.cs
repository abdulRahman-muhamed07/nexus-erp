using System.ComponentModel.DataAnnotations;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Activity
{
 public int Id { get; set; }
 [MaxLength(60)] public string Icon { get; set; } = string.Empty;
 [MaxLength(400)] public string Text { get; set; } = string.Empty;
 public DateTime Time { get; set; } = DateTime.UtcNow;
}