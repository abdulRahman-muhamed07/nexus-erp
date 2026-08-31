using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class PaymentRepository(XeoTechDbContext db) : IPaymentRepository
{
    public async Task<IReadOnlyList<Payment>> GetByOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
        => await db.Payments
            .AsNoTracking()
            .Where(x => x.OrderId == orderId)
            .OrderByDescending(x => x.Date)
            .ToListAsync(cancellationToken);

    public Task<Order?> GetOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
        => db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

    public async Task<decimal> GetPaidAmountAsync(
        int orderId,
        CancellationToken cancellationToken = default)
        => await db.Payments
            .Where(x => x.OrderId == orderId)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;

    public void Add(Payment payment) => db.Payments.Add(payment);
}
