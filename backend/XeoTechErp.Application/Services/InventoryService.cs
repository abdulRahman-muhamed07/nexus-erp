using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Common;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Services;

public interface IInventoryService
{
    Task<object> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<Result> AdjustAsync(int productId, int delta, string reason, string actor, CancellationToken cancellationToken = default);
}

public sealed class InventoryService(IProductRepository products, IInventoryRepository inventory, IUnitOfWork unitOfWork) : IInventoryService
{
    public async Task<object> GetSummaryAsync(CancellationToken cancellationToken = default) => await inventory.GetSummaryAsync(cancellationToken);

    public async Task<Result> AdjustAsync(int productId, int delta, string reason, string actor, CancellationToken cancellationToken = default)
    {
        var product = await products.GetByIdAsync(productId, cancellationToken);
        if (product is null) return Result.Failure("PRODUCT_NOT_FOUND", "Product was not found.");
        if (product.Stock + delta < 0) return Result.Failure("NEGATIVE_STOCK", "Stock cannot become negative.");
        product.Stock += delta;
        inventory.AddMovement(new StockMovement { ProductId = product.Id, ProductName = product.Name, Delta = delta, Reason = string.IsNullOrWhiteSpace(reason) ? "Manual Adjustment" : reason.Trim(), By = actor });
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
