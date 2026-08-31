using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Contracts.PurchaseOrders;

public sealed record PurchaseOrderResponse(int Id, int SupplierId, int ProductId, string ProductName, int Qty, decimal Cost, PoStatus Status, DateTime Eta, DateTime Created);
