using System.ComponentModel.DataAnnotations;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class Notification
{
 public int Id { get; set; }
 [MaxLength(60)] public string Icon { get; set; } = string.Empty;
 [MaxLength(160)] public string Title { get; set; } = string.Empty;
 [MaxLength(400)] public string Description { get; set; } = string.Empty;
 public DateTime Time { get; set; } = DateTime.UtcNow;
 public bool IsRead { get; set; }
}