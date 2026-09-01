namespace XeoTechErp.Application.Common.Models;

public sealed record InventorySummaryResponse(
    int Products,
    int Units,
    decimal InventoryValue,
    int LowStock);
