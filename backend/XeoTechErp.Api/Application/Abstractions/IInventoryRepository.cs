using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Application.Abstractions;

public interface IInventoryRepository
{
    Task<object> GetSummaryAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetProductAsync(int productId, CancellationToken cancellationToken = default);
    void AddMovement(StockMovement movement);
}