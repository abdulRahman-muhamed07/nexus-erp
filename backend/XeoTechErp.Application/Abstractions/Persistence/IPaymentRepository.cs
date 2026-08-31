using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IPaymentRepository
{
    Task<IReadOnlyList<Payment>> GetByOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<Order?> GetOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<decimal> GetPaidAmountAsync(int orderId, CancellationToken cancellationToken = default);
    void Add(Payment payment);
}
