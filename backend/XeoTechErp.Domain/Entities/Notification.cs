namespace XeoTechErp.Domain.Entities;

public sealed class Notification
{
    public int Id { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Time { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}
