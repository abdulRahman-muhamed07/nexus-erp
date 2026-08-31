using System.ComponentModel.DataAnnotations;
namespace XeoTechErp.Api.Domain.Entities;
public sealed class RefreshToken
{
 public int Id { get; set; }
 public int UserId { get; set; }
 public User User { get; set; } = null!;
 [Required,MaxLength(200)] public string TokenHash { get; set; } = string.Empty;
 public DateTime ExpiresAt { get; set; }
 public DateTime? RevokedAt { get; set; }
 public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}