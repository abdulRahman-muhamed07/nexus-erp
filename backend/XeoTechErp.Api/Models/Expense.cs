using System.ComponentModel.DataAnnotations;

namespace XeoTechErp.Api.Models;

public sealed class Expense
{
    public int Id { get; set; }
    [Required, MaxLength(60)] public string Category { get; set; } = "";
    public decimal Amount { get; set; }
    public DateTime Date { get; set; } = DateTime.UtcNow;
    [MaxLength(500)] public string Description { get; set; } = "";
}

public sealed class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    [Required, MaxLength(200)] public string TokenHash { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsActive => RevokedAt is null && ExpiresAt > DateTime.UtcNow;
}
