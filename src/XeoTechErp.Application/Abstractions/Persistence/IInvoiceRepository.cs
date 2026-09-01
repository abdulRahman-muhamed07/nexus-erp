using XeoTechErp.Domain.Entities;
using XeoTechErp.Domain.Enums;

namespace XeoTechErp.Application.Abstractions.Persistence;

public interface IInvoiceRepository
{
    Task<IReadOnlyList<Invoice>> GetAllAsync(InvoiceStatus? status, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Order?> GetDeliveredOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<bool> ExistsForOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task<decimal> GetOrderPaymentsAsync(int orderId, CancellationToken cancellationToken = default);
    void AddInvoice(Invoice invoice);
    void AddPayment(Payment payment);
}