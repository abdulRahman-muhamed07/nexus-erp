using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IReturnRepository
{
    Task<IReadOnlyList<Return>> GetAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetDeliveredOrderWithItemsAsync(int orderId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForOrderAsync(int orderId, CancellationToken cancellationToken = default);
    void Add(Return @return);
}
