using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Application.Abstractions;

public interface IOrderRepository
{
    Task<Customer?> GetCustomerAsync(int customerId, CancellationToken cancellationToken = default);
    Task<Dictionary<int, Product>> GetProductsAsync(IReadOnlyCollection<int> productIds, CancellationToken cancellationToken = default);
    Task<AppConfig?> GetConfigurationAsync(CancellationToken cancellationToken = default);
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    void AddStockMovement(StockMovement movement);
    void AddAuditLog(AuditLogEntry auditLog);
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default);
}