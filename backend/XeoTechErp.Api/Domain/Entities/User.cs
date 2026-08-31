using System.ComponentModel.DataAnnotations;
using XeoTechErp.Api.Domain.Enums;

namespace XeoTechErp.Api.Domain.Entities;

public sealed class User
{
    public int Id { get; set; }
    [Required, MaxLength(120)] public string Email { get; set; } = null!;
    [Required, MaxLength(255)] public string PasswordHash { get; set; } = null!;
    public Role Role { get; set; } = Role.Viewer;
    [MaxLength(120)] public string DisplayName { get; set; } = string.Empty;
}
