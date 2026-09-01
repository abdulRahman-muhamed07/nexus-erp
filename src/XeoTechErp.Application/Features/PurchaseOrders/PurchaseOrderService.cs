using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.PurchaseOrders;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Services;

public sealed class PurchaseOrderService(IPurchaseOrderRepository repository, IInventoryRepository inventory, IUnitOfWork unitOfWork, IMapper mapper) : IPurchaseOrderService
{
    public async Task<IReadOnlyList<PurchaseOrderResponse>> GetAsync(CancellationToken cancellationToken = default)
        => mapper.Map<IReadOnlyList<PurchaseOrderResponse>>(await repository.GetAsync(cancellationToken));

    public async Task<Result<PurchaseOrderResponse>> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.SupplierId <= 0 || request.ProductId <= 0 || request.Qty <= 0 || request.Cost < 0)
            return Result<PurchaseOrderResponse>.Failure("PURCHASE_ORDER_INVALID", "Supplier, product, positive quantity and non-negative cost are required.");
        if (!await repository.SupplierExistsAsync(request.SupplierId, cancellationToken))
            return Result<PurchaseOrderResponse>.Failure("SUPPLIER_NOT_FOUND", "Supplier was not found.");
        var product = await repository.GetProductAsync(request.ProductId, cancellationToken);
        if (product is null) return Result<PurchaseOrderResponse>.Failure("PRODUCT_NOT_FOUND", "Product was not found.");

        var purchaseOrder = new PurchaseOrder { SupplierId = request.SupplierId, ProductId = product.Id, ProductName = product.Name, Qty = request.Qty, Cost = request.Cost, Eta = request.Eta == default ? DateTime.UtcNow : request.Eta, Status = PoStatus.Pending };
        repository.Add(purchaseOrder);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<PurchaseOrderResponse>.Success(mapper.Map<PurchaseOrderResponse>(purchaseOrder));
    }

    public async Task<Result<PurchaseOrderResponse>> UpdateStatusAsync(int id, PoStatus status, CancellationToken cancellationToken = default)
    {
        var purchaseOrder = await repository.GetByIdAsync(id, cancellationToken);
        if (purchaseOrder is null) return Result<PurchaseOrderResponse>.Failure("PURCHASE_ORDER_NOT_FOUND", "Purchase order was not found.");
        if (purchaseOrder.Status == PoStatus.Received && status == PoStatus.Received)
            return Result<PurchaseOrderResponse>.Failure("PURCHASE_ORDER_INVALID", "Purchase order is already received.");

        purchaseOrder.Status = status;
        if (status == PoStatus.Received)
        {
            var product = await repository.GetProductAsync(purchaseOrder.ProductId, cancellationToken);
            if (product is null) return Result<PurchaseOrderResponse>.Failure("PRODUCT_NOT_FOUND", "Product was not found.");
            var currentValue = product.Stock * product.Cost;
            var incomingValue = purchaseOrder.Qty * purchaseOrder.Cost;
            var newCost = (currentValue + incomingValue) / Math.Max(1, product.Stock + purchaseOrder.Qty);
            product.SetDetails(product.Sku, product.Name, product.Price, newCost, product.ReorderLevel, product.Category, product.SupplierId);
            product.IncreaseStock(purchaseOrder.Qty);
            inventory.AddMovement(new StockMovement { ProductId = product.Id, ProductName = product.Name, Delta = purchaseOrder.Qty, Reason = "Purchase", Reference = $"PO:{purchaseOrder.Id}" });
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<PurchaseOrderResponse>.Success(mapper.Map<PurchaseOrderResponse>(purchaseOrder));
    }
}
