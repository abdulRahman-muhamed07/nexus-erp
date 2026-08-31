using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IInventoryRepository
{
    Task<(int Products, int Units, decimal InventoryValue, int LowStock)> GetSummaryAsync(CancellationToken cancellationToken = default);
    void AddMovement(StockMovement movement);
}
