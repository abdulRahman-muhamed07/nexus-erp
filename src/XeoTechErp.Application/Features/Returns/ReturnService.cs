using AutoMapper;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Contracts.Returns;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Services;

public sealed class ReturnService(
    IReturnRepository repository,
    IProductRepository products,
    IInventoryRepository inventory,
    IUnitOfWork unitOfWork,
    IMapper mapper) : IReturnService
{
    public async Task<IReadOnlyList<ReturnResponse>> GetAsync(CancellationToken cancellationToken = default)
        => mapper.Map<IReadOnlyList<ReturnResponse>>(await repository.GetAsync(cancellationToken));

    public async Task<Result<ReturnResponse>> CreateAsync(CreateReturnRequest request, CancellationToken cancellationToken = default)
    {
        if (request.OrderId <= 0 || request.Amount <= 0 || string.IsNullOrWhiteSpace(request.Reason))
            return Result<ReturnResponse>.Failure("RETURN_INVALID", "Order, positive amount and reason are required.");

        var order = await repository.GetDeliveredOrderWithItemsAsync(request.OrderId, cancellationToken);
        if (order is null) return Result<ReturnResponse>.Failure("ORDER_NOT_FOUND", "A delivered order was not found.");
        if (await repository.ExistsForOrderAsync(order.Id, cancellationToken))
            return Result<ReturnResponse>.Failure("RETURN_EXISTS", "This order has already been returned.");
        if (request.Amount != order.Total)
            return Result<ReturnResponse>.Failure("PARTIAL_RETURN_UNSUPPORTED", "Only full-order returns are supported.");

        var result = new Return
        {
            OrderId = order.Id,
            Amount = request.Amount,
            Reason = request.Reason.Trim(),
            Date = DateTime.UtcNow
        };

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            repository.Add(result);
            foreach (var item in order.Items)
            {
                var product = await products.GetByIdAsync(item.ProductId, cancellationToken);
                if (product is null) continue;
                product.IncreaseStock(item.Qty);
                inventory.AddMovement(new StockMovement { ProductId = product.Id, ProductName = product.Name, Delta = item.Qty, Reason = "Return", Reference = $"Order:{order.Id}" });
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return Result<ReturnResponse>.Success(mapper.Map<ReturnResponse>(result));
    }
}
