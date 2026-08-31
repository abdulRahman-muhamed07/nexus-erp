namespace XeoTechErp.Application.Contracts.PurchaseOrders;

public sealed record CreatePurchaseOrderRequest(int SupplierId, int ProductId, int Qty, decimal Cost, DateTime Eta);
