namespace XeoTechErp.Domain.Entities;

public sealed class StockMovement
{
    public int Id { get; set; }
    public int? ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Delta { get; set; }
    public string Reason { get; set; } = "Adjustment";
    public string? Reference { get; set; }
    public string By { get; set; } = string.Empty;
    public DateTime Time { get; set; } = DateTime.UtcNow;
}
