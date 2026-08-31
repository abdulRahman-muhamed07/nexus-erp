using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Application.Services;

public interface IInventoryService
{
    Task<object> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<bool> AdjustAsync(int productId, int delta, string reason, string actor, CancellationToken cancellationToken = default);
}

public sealed class InventoryService(IInventoryRepository repository, IUnitOfWork unitOfWork) : IInventoryService
{
    public Task<object> GetSummaryAsync(CancellationToken cancellationToken = default) =>
        repository.GetSummaryAsync(cancellationToken);

    public async Task<bool> AdjustAsync(int productId, int delta, string reason, string actor, CancellationToken cancellationToken = default)
    {
        var product = await repository.GetProductAsync(productId, cancellationToken);
        if (product is null)
            return false;

        if (product.Stock + delta < 0)
            throw new InvalidOperationException("Stock cannot become negative.");

        product.Stock += delta;
        repository.AddMovement(new StockMovement
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Delta = delta,
            Reason = reason,
            By = actor
        });

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }
}