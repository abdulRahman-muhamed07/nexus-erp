using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.PurchaseOrders;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Services;

public interface IPurchaseOrderService
{
    Task<IReadOnlyList<PurchaseOrderResponse>> GetAsync(CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderResponse>> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<PurchaseOrderResponse>> UpdateStatusAsync(int id, PoStatus status, CancellationToken cancellationToken = default);
}
