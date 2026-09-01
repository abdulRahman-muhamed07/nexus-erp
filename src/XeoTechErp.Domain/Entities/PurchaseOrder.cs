using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Domain.Entities;

public sealed class PurchaseOrder
{
    public int Id { get; set; }
    public int SupplierId { get; set; }
    public Supplier Supplier { get; set; } = null!;
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Qty { get; set; }
    public decimal Cost { get; set; }
    public PoStatus Status { get; set; } = PoStatus.Pending;
    public DateTime Eta { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
}
