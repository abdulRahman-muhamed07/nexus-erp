using Microsoft.EntityFrameworkCore;
using XeoTechErp.Application.Abstractions.Persistence;
using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Infrastructure.Persistence.Repositories;

public sealed class InvoiceRepository(XeoTechDbContext db) : IInvoiceRepository
{
    public async Task<IReadOnlyList<Invoice>> GetAllAsync(InvoiceStatus? status, CancellationToken cancellationToken = default)
    {
        IQueryable<Invoice> query = db.Invoices
            .AsNoTracking()
            .Include(x => x.Customer)
            .Include(x => x.Order);

        if (status.HasValue)
            query = query.Where(x => x.Status == status.Value);

        return await query.OrderByDescending(x => x.Issued).ToListAsync(cancellationToken);
    }

    public Task<Invoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Invoices.Include(x => x.Customer).Include(x => x.Order)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Order?> GetDeliveredOrderAsync(int orderId, CancellationToken cancellationToken = default) =>
        db.Orders.Include(x => x.Customer).FirstOrDefaultAsync(x => x.Id == orderId, cancellationToken);

    public Task<bool> ExistsForOrderAsync(int orderId, CancellationToken cancellationToken = default) =>
        db.Invoices.AnyAsync(x => x.OrderId == orderId, cancellationToken);

    public async Task<decimal> GetOrderPaymentsAsync(int orderId, CancellationToken cancellationToken = default) =>
        await db.Payments.Where(x => x.OrderId == orderId)
            .SumAsync(x => (decimal?)x.Amount, cancellationToken) ?? 0m;

    public void AddInvoice(Invoice invoice) => db.Invoices.Add(invoice);
    public void AddPayment(Payment payment) => db.Payments.Add(payment);
}