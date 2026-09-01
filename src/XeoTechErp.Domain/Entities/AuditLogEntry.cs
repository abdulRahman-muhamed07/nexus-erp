namespace XeoTechErp.Domain.Entities;

public sealed class AuditLogEntry
{
    public int Id { get; set; }
    public DateTime Time { get; set; } = DateTime.UtcNow;
    public string User { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string Module { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}
