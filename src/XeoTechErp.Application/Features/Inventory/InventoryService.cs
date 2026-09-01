using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Application.Abstractions.Services;
using XeoTechErp.Application.Common;
using XeoTechErp.Application.Common.Models;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Exceptions;

namespace XeoTechErp.Application.Services;

public sealed class InventoryService(
    IProductRepository products,
    IInventoryRepository inventory,
    IUnitOfWork unitOfWork) : IInventoryService
{
    public async Task<InventorySummaryResponse> GetSummaryAsync(CancellationToken cancellationToken = default)
    {
        var summary = await inventory.GetSummaryAsync(cancellationToken);
        return new InventorySummaryResponse(
            summary.Products,
            summary.Units,
            summary.InventoryValue,
            summary.LowStock);
    }

    public async Task<Result> AdjustAsync(
        int productId,
        int delta,
        string reason,
        string actor,
        CancellationToken cancellationToken = default)
    {
        if (delta == 0)
            return Result.Failure("INVALID_ADJUSTMENT", "Stock adjustment cannot be zero.");

        var product = await products.GetByIdAsync(productId, cancellationToken);
        if (product is null)
            return Result.Failure("PRODUCT_NOT_FOUND", "Product was not found.");

        try
        {
            if (delta > 0)
                product.IncreaseStock(delta);
            else
                product.DecreaseStock(Math.Abs(delta));
        }
        catch (DomainRuleException ex)
        {
            return Result.Failure("INVALID_STOCK_ADJUSTMENT", ex.Message);
        }

        inventory.AddMovement(new StockMovement
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Delta = delta,
            Reason = string.IsNullOrWhiteSpace(reason) ? "Manual Adjustment" : reason.Trim(),
            By = string.IsNullOrWhiteSpace(actor) ? "system" : actor.Trim()
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
