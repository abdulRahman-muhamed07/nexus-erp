namespace XeoTechErp.Domain.Entities;

public sealed class Activity
{
    public int Id { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public DateTime Time { get; set; } = DateTime.UtcNow;
}
