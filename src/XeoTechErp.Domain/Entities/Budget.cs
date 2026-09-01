namespace XeoTechErp.Domain.Entities;

public sealed class Budget
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal MonthlyAmount { get; set; }
}
