using Microsoft.EntityFrameworkCore;
using XeoTechErp.Api.Application.Abstractions;
using XeoTechErp.Api.Domain.Entities;

namespace XeoTechErp.Api.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(XeoTechDbContext db) : IOrderRepository
{
    public Task<Customer?> GetCustomerAsync(int customerId, CancellationToken cancellationToken = default) =>
        db.Customers.SingleOrDefaultAsync(customer => customer.Id == customerId, cancellationToken);

    public async Task<Dictionary<int, Product>> GetProductsAsync(IReadOnlyCollection<int> productIds, CancellationToken cancellationToken = default) =>
        await db.Products.Where(product => productIds.Contains(product.Id)).ToDictionaryAsync(product => product.Id, cancellationToken);

    public Task<AppConfig?> GetConfigurationAsync(CancellationToken cancellationToken = default) =>
        db.AppConfig.AsNoTracking().FirstOrDefaultAsync(cancellationToken);

    public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        db.Orders.AsNoTracking().SingleOrDefaultAsync(order => order.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Orders.AsNoTracking().OrderByDescending(order => order.OrderDate).ToListAsync(cancellationToken);

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        db.Orders.Add(order);
        return Task.CompletedTask;
    }

    public void AddStockMovement(StockMovement movement) => db.StockMovements.Add(movement);

    public void AddAuditLog(AuditLogEntry auditLog) => db.AuditLog.Add(auditLog);

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        await action();
        await transaction.CommitAsync(cancellationToken);
    }
}